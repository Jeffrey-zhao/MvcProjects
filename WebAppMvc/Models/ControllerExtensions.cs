using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Collections;
using System.Reflection;

namespace WebAppMvc.Models
{
    public static class ControllerExtensions
    {
        public static object InvokeAction(this Controller controller, string actionName)
        {
            IModelBinder modelBinder = new MyDefaultModelBinder();
            ControllerDescriptor controllerDescriptor = new ReflectedControllerDescriptor(controller.GetType());
            ActionDescriptor actionDescriptor = controllerDescriptor.FindAction(controller.ControllerContext, actionName);
            Dictionary<string, object> arguments = new Dictionary<string, object>();
            foreach (ParameterDescriptor parameterDescriptor in actionDescriptor.GetParameters())
            {
                string modelName = parameterDescriptor.BindingInfo.Prefix ?? parameterDescriptor.ParameterName;
                ModelBindingContext bindingContext = new ModelBindingContext
                {
                    FallbackToEmptyPrefix = parameterDescriptor.BindingInfo.Prefix == null,
                    ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, parameterDescriptor.ParameterType),
                    ModelName = modelName,
                    ModelState = controller.ModelState,
                    ValueProvider = controller.ValueProvider
                };
                object argument = modelBinder.BindModel(controller.ControllerContext, bindingContext);
                arguments.Add(parameterDescriptor.ParameterName, argument);
            }
            return actionDescriptor.Execute(controller.ControllerContext, arguments);
        }
    }

    public class MyDefaultModelBinder : IModelBinder
    {
        private IEnumerable<string> GetIndexs(ControllerContext controllerContext, ModelBindingContext bindingContext, out bool isZeroBased)
        {
            string key = (bindingContext.ModelName ?? "") + ".index";
            key = key.Trim('.');
            ValueProviderResult valueProviderResult = bindingContext.ValueProvider.GetValue(key);
            if (null != valueProviderResult)
            {
                isZeroBased = false;
                return (string[])valueProviderResult.ConvertTo(typeof(string[]));
            }
            isZeroBased = true;
            return getZeroBasedIndexes();
        }

        private static IEnumerable<string> getZeroBasedIndexes()
        {
            int index = 0;
            while (true)
            {
                yield return (index++).ToString();
            }
        }
        private static void CopyDictionary<TKey, TValue>(IDictionary<TKey, TValue> destination, IEnumerable<KeyValuePair<object, object>> source)
        {
            foreach (KeyValuePair<object, object> item in source)
            {
                if (item.Key is TKey)
                {
                    destination.Add((TKey)item.Key, item.Value is TValue ? (TValue)item.Value : default(TValue));
                }
            }
        }

        private static void CopyCollection<T>(ICollection<T> destination, IEnumerable source)
        {
            foreach (object item in source)
            {
                destination.Add(item is T ? (T)item : default(T));
            }
        }
        private void Copy(Type elementType, object destinction, object source)
        {
            MethodInfo copyMethod = typeof(MyDefaultModelBinder).GetMethod("CopyCollection", BindingFlags.Static | BindingFlags.NonPublic);
            copyMethod.MakeGenericMethod(elementType).Invoke(null, new object[] { destinction, source });
        }
        private void Copy(Type keyType, Type valueType, object destinction, IEnumerable<KeyValuePair<object, object>> source)
        {
            MethodInfo copyMethod = typeof(MyDefaultModelBinder).GetMethod("CopyDictionary", BindingFlags.Static | BindingFlags.NonPublic);
            copyMethod.MakeGenericMethod(keyType, valueType).Invoke(null, new object[] { destinction, source });
        }
        private bool Match(Type type, Type typeToMatch)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeToMatch)
            {
                return true;
            }
            foreach (Type interfaceType in type.GetInterfaces())
            {
                if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeToMatch)
                {
                    return true;
                }
            }
            return false;
        }
        private object CreateModel(ControllerContext controllerContext, ModelBindingContext bindingContext, Type modelType)
        {
            if (modelType.IsGenericType)
            {
                Type genericTypeDefinition = modelType.GetGenericTypeDefinition();
                if (typeof(IDictionary<,>) == genericTypeDefinition)
                {
                    return Activator.CreateInstance(typeof(Dictionary<,>).MakeGenericType(modelType.GetGenericArguments()));
                }
                if (new Type[] { typeof(IEnumerable<>), typeof(ICollection<>), typeof(IList<>) }.Any(type => type == genericTypeDefinition))
                {
                    return Activator.CreateInstance(typeof(List<>).MakeGenericType(modelType.GetGenericArguments()));
                }
            }
            return Activator.CreateInstance(modelType);
        }
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            string key = bindingContext.ModelName;
            IValueProvider valueProvider = bindingContext.ValueProvider;
            bool containPrefix = valueProvider.ContainsPrefix(key);
            if (!containPrefix)
            {
                if (!bindingContext.FallbackToEmptyPrefix)
                {
                    return null;
                }
                bindingContext.ModelName = null;
            }
            else
            {
                ValueProviderResult valueProviderResult = valueProvider.GetValue(key);
                if (null != valueProviderResult)
                {
                    return this.BindSampleModel(controllerContext, bindingContext, valueProviderResult);
                }
            }
            if (bindingContext.ModelMetadata.IsComplexType)
            {
                return this.BindComplexModel(controllerContext, bindingContext);
            }
            return null;
        }

        private object BindSampleModel(ControllerContext controllerContext, ModelBindingContext bindingContext, ValueProviderResult valueProviderResult)
        {
            SetModelState(bindingContext, valueProviderResult);
            Type modelType = bindingContext.ModelType;
            if (typeof(string) != modelType && this.Match(modelType, typeof(IEnumerable<>)))
            {
                Type arrayType = modelType.IsArray ? modelType : modelType.GetGenericArguments()[0].MakeArrayType();
                object array = valueProviderResult.ConvertTo(arrayType);
                if (bindingContext.ModelType.IsArray)
                {
                    return array;
                }
                object list = this.CreateModel(controllerContext, bindingContext, modelType);
                this.Copy(modelType.GetGenericArguments()[0], list, array);
                return list;
            }
            return valueProviderResult.ConvertTo(bindingContext.ModelType);
        }

        private object BindComplexModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            Type modelType = bindingContext.ModelType;
            if (this.Match(modelType, typeof(IDictionary<,>)))
            {
                return this.BindDicionary(controllerContext, bindingContext);
            }
            if (this.Match(modelType, typeof(IEnumerable<>)))
            {
                return this.BindCollection(controllerContext, bindingContext);
            }
            object model = this.CreateModel(controllerContext, bindingContext, modelType);
            bindingContext.ModelMetadata.Model = model;

            ICustomTypeDescriptor modelTypeDescriptor = new AssociatedMetadataTypeTypeDescriptionProvider(modelType)
                .GetTypeDescriptor(modelType);
            PropertyDescriptorCollection propertyDescriptors = modelTypeDescriptor.GetProperties();
            foreach (PropertyDescriptor item in propertyDescriptors)
            {
                this.BindProperty(controllerContext, bindingContext, item);
            }
            return model;
        }

        private void BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor item)
        {
            string prefix = (bindingContext.ModelName ?? "") + "." + (item.Name ?? "");
            prefix = prefix.Trim('.');
            ModelMetadata metadata = bindingContext.PropertyMetadata[item.Name];
            ModelBindingContext context = new ModelBindingContext
            {
                ModelName = prefix,
                ModelMetadata = metadata,
                ModelState = bindingContext.ModelState,
                ValueProvider = bindingContext.ValueProvider
            };

            object propertyValue = ModelBinders.Binders.GetBinder(item.PropertyType).BindModel(controllerContext, context);
            if (bindingContext.ModelMetadata.ConvertEmptyStringToNull && object.Equals(propertyValue, string.Empty))
            {
                propertyValue = null;
            }
            context.ModelMetadata.Model = propertyValue;
            item.SetValue(bindingContext.Model, propertyValue);
        }

        private object BindCollection(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            Type modelType = bindingContext.ModelType;
            Type elementType = modelType.IsArray ? modelType.GetElementType() : modelType.GetGenericArguments()[0];

            Type collectionType = modelType.IsArray ? typeof(List<>).MakeGenericType(elementType) : modelType;
            object model = this.CreateModel(controllerContext, bindingContext, collectionType);
            bindingContext.ModelMetadata.Model = model;
            bool isZeroBased;
            IEnumerable<string> indexes = this.GetIndexs(controllerContext, bindingContext, out isZeroBased);
            List<object> elements = new List<object>();
            foreach (string index in indexes)
            {
                string prefix = string.Format("{0}[{1}]", bindingContext.ModelName, index);
                if (!bindingContext.ValueProvider.ContainsPrefix(prefix))
                {
                    if (isZeroBased)
                    {
                        break;
                    }
                    continue;
                }
                ModelBindingContext context = new ModelBindingContext
                {
                    ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, elementType),
                    ModelName = prefix,
                    ModelState = bindingContext.ModelState,
                    PropertyFilter = bindingContext.PropertyFilter,
                    ValueProvider = bindingContext.ValueProvider
                };
                object element = ModelBinders.Binders.GetBinder(elementType).BindModel(controllerContext, context);
                elements.Add(element);
            }
            this.Copy(elementType, bindingContext.Model, elements);
            if (modelType.IsArray)
            {
                IList list = model as IList;
                if (null == list)
                {
                    return null;
                }
                Array array = Array.CreateInstance(elementType, list.Count);
                list.CopyTo(array, 0);
                return array;
            }
            return model;
        }

        private object BindDicionary(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            Type modelType = bindingContext.ModelType;
            Type keyType = modelType.GetGenericArguments()[0];
            Type valueType = modelType.GetGenericArguments()[1];

            object model = this.CreateModel(controllerContext, bindingContext, modelType);
            bindingContext.ModelMetadata.Model = model;
            List<KeyValuePair<object, object>> list = new List<KeyValuePair<object, object>>();
            bool isZeroBased;
            IEnumerable<string> indexes = this.GetIndexs(controllerContext, bindingContext, out isZeroBased);
            foreach (string index in indexes)
            {
                string prefix = string.Format("{0}[{1}]", bindingContext.ModelName, index);
                if (!bindingContext.ValueProvider.ContainsPrefix(prefix))
                {
                    if (isZeroBased)
                    {
                        break;
                    }
                    continue;
                }
                ModelBindingContext contextForKey = new ModelBindingContext
                {
                    ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, keyType),
                    ModelName = prefix+".key",
                    ModelState = bindingContext.ModelState,
                    PropertyFilter = bindingContext.PropertyFilter,
                    ValueProvider = bindingContext.ValueProvider
                };

                ModelBindingContext contextForValue = new ModelBindingContext
                {
                    ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, valueType),
                    ModelName = prefix+".value",
                    ModelState = bindingContext.ModelState,
                    PropertyFilter = bindingContext.PropertyFilter,
                    ValueProvider = bindingContext.ValueProvider
                };
                object key = ModelBinders.Binders.GetBinder(keyType).BindModel(controllerContext, contextForKey);
                object value = ModelBinders.Binders.GetBinder(valueType).BindModel(controllerContext, contextForValue);
                list.Add(new KeyValuePair<object, object>(key,value));
            }
            this.Copy(keyType,valueType, bindingContext.Model, list);
            return model;
        }
        private void SetModelState(ModelBindingContext bindingContext, ValueProviderResult valueProviderResult)
        {
            ModelState modelState;
            if (!bindingContext.ModelState.TryGetValue(bindingContext.ModelName, out modelState))
            {
                bindingContext.ModelState.Add(bindingContext.ModelName, modelState = new ModelState());
            }
            modelState.Value = valueProviderResult;
        }
    }
}