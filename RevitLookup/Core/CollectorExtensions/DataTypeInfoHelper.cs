// Copyright 2003-2021 by Autodesk, Inc. 
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted, 
// provided that the above copyright notice appears in all copies and 
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting 
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS. 
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC. 
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
//
// Use, duplication, or disclosure by the U.S. Government is subject to 
// restrictions set forth in FAR 52.227-19 (Commercial Computer
// Software - Restricted Rights) and DFAR 252.227-7013(c)(1)(ii)
// (Rights in Technical Data and Computer Software), as applicable.

using System.Collections;
using System.Reflection;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Visual;
using RevitLookup.Core.RevitTypes;
using RevitLookup.Core.RevitTypes.PlaceHolders;
using AssetProperty = Autodesk.Revit.DB.Visual.AssetProperty;
using CategoryNameMap = Autodesk.Revit.DB.CategoryNameMap;
using Color = Autodesk.Revit.DB.Color;
using Double = RevitLookup.Core.RevitTypes.Double;
using DoubleArray = Autodesk.Revit.DB.DoubleArray;
using ElementId = Autodesk.Revit.DB.ElementId;
using ElementSet = Autodesk.Revit.DB.ElementSet;
using Enumerable = RevitLookup.Core.RevitTypes.Enumerable;
using Exception = System.Exception;
using Object = RevitLookup.Core.RevitTypes.Object;
using ParameterSet = Autodesk.Revit.DB.ParameterSet;
using String = RevitLookup.Core.RevitTypes.String;

namespace RevitLookup.Core.CollectorExtensions;

public static class DataTypeInfoHelper
{
    public static Data CreateFrom(Document document, MethodInfo info, object returnValue, object elem)
    {
        return CreateFrom(document, info, info.ReturnType, returnValue, elem);
    }

    private static Data CreateFrom(Document document, MemberInfo info, Type expectedType, object returnValue, object elem)
    {
        try
        {
            if (expectedType == typeof(bool))
            {
                var val = returnValue as bool?;
                return new Bool(info.Name, val.GetValueOrDefault());
            }

            if (expectedType == typeof(CategoryNameMap))
                return new RevitTypes.CategoryNameMap(info.Name, returnValue as CategoryNameMap);

            if (expectedType == typeof(double))
                return new Double(info.Name, (double) returnValue);

            if (expectedType == typeof(double?))
            {
                var value = (double?) returnValue;
                if (value.HasValue) return new Double(info.Name, value.Value);
                return new EmptyValue(info.Name);
            }

            if (expectedType == typeof(byte))
                return new Int(info.Name, (byte) returnValue);

            if ((expectedType == typeof(GeometryObject) || expectedType == typeof(GeometryElement)) && elem is Element element)
                return new ElementGeometry(info.Name, element, document.Application);

            if (expectedType == typeof(ElementId))
            {
                if (info.Name == nameof(Element.Id))
                    return new String(info.Name, (returnValue as ElementId)?.IntegerValue.ToString());

                return new RevitTypes.ElementId(info.Name, returnValue as ElementId, document);
            }

            if (expectedType == typeof(ElementSet))
                return new RevitTypes.ElementSet(info.Name, returnValue as ElementSet);

            if (expectedType == typeof(AssetProperty))
                return new RevitTypes.AssetProperty(info.Name, elem as AssetProperties);

            if (expectedType == typeof(Color))
                return new RevitTypes.Color(info.Name, returnValue as Color);

            if (expectedType == typeof(DoubleArray))
                return new RevitTypes.DoubleArray(info.Name, returnValue as DoubleArray);

            if (expectedType == typeof(int))
            {
                var val = returnValue as int?;
                return new Int(info.Name, val.GetValueOrDefault());
            }

            if (expectedType == typeof(ParameterSet))
                return new RevitTypes.ParameterSet(info.Name, elem as Element, returnValue as ParameterSet);

            if (expectedType == typeof(string))
                return new String(info.Name, returnValue as string);

            if (expectedType == typeof(UV))
                return new Uv(info.Name, returnValue as UV);

            if (expectedType == typeof(XYZ))
                return new Xyz(info.Name, returnValue as XYZ);

            if (expectedType == typeof(PlanTopologySet))
            {
                var set = (PlanTopologySet) returnValue;
                var placeholders = set
                    .Cast<PlanTopology>()
                    .Select(x => new PlanTopologyPlaceholder(x))
                    .ToList();
                return new Enumerable(info.Name, placeholders, document);
            }

            if (typeof(IEnumerable).IsAssignableFrom(expectedType)
                && expectedType.IsGenericType
                && (expectedType.GenericTypeArguments[0] == typeof(double)
                    || expectedType.GenericTypeArguments[0] == typeof(int))
                || expectedType == typeof(DoubleArray))
                return new EnumerableAsString(info.Name, returnValue as IEnumerable);

            if (typeof(IEnumerable).IsAssignableFrom(expectedType))
                return new Enumerable(info.Name, returnValue as IEnumerable, document);

            if (expectedType.IsEnum)
                return new String(info.Name, returnValue.ToString());

            if (expectedType == typeof(Guid))
            {
                var guidValue = (Guid) returnValue;
                return new String(info.Name, guidValue.ToString());
            }

            return new Object(info.Name, returnValue);
        }
        catch (Exception ex)
        {
            return new RevitTypes.Exception(info.Name, ex);
        }
    }

    public static void AddDataFromTypeInfo(Document document, MemberInfo info, Type expectedType, object returnValue, object elem, ArrayList data)
    {
        data.Add(CreateFrom(document, info, expectedType, returnValue, elem));
    }
}