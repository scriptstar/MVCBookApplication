using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace MvcBookApplication.Models
{
    //class GenericBinder : IModelBinder
    //{
    //    public object GetValue(ControllerContext controllerContext,
    //                            string modelName, Type modelType,
    //                            ModelStateDictionary modelState)
    //    {
    //        var instance = Activator.CreateInstance(modelType);
    //        var propertyInfos = modelType.GetProperties();

    //        foreach (var prop in propertyInfos)
    //        {
    //            object value = null;
    //            if (controllerContext.HttpContext.Request
    //                                    .Form[prop.Name.ToLower()] != null)
    //            {
    //                value = controllerContext.HttpContext.Request
    //                    .Form[prop.Name.ToLower()];
    //            }else if (controllerContext.HttpContext.Request
    //                                    .QueryString[prop.Name.ToLower()] != null)
    //            {
    //                value = controllerContext.HttpContext.Request
    //                    .QueryString[prop.Name.ToLower()];
    //            }

    //            prop.SetValue(instance,value,
    //                    null);
    //        }
    //        return instance;
    //    }
    //}

    //class QueryStringBinder : IModelBinder
    //{
    //    public object GetValue(ControllerContext controllerContext,
    //                            string modelName, Type modelType,
    //                            ModelStateDictionary modelState)
    //    {
    //        var instance = Activator.CreateInstance(modelType);
    //        PropertyInfo[] propertyInfos;
    //        propertyInfos = modelType.GetProperties();

    //        foreach (var prop in propertyInfos)
    //        {
    //            prop.SetValue(instance,
    //                    controllerContext.HttpContext.Request
    //                                    .QueryString[prop.Name.ToLower()],
    //                    null);
    //        }
    //        return instance;
    //    }
    //}
}
