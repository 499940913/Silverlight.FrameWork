
//using System;

//namespace BaseTool
//{
//    public static class BingdingHelper
//    {
//        public static void Bingding(object target, object binding)
//        {
//            var properties = binding.GetType().GetProperties();
//            foreach (var property in properties)
//            {
//                var v = property.GetValue(binding, null);
//                if (v != null)
//                {
//                    var p = target.GetType().GetProperty(property.Name);
//                    if (p != null && p.PropertyType == property.PropertyType)
//                    {
//                        p.SetValue(target, v, null);
//                    }
//                }
//            }
//        }
//    }
//}
