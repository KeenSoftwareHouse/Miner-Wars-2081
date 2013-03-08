using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Entities.Prefabs;
using MinerWarsMath;

namespace MinerWars.AppCode.Game.Prefabs
{
    /// <summary>
    /// Snap point
    /// </summary>
    class MyPrefabSnapPoint
    {
        /// <summary>
        /// Helper class for snap point type and contraints
        /// </summary>
        public class MyPrefabSnapPointType
        {
            public BuildTypesEnum? BuildType;
            public CategoryTypesEnum? CategoryType;
            public SubCategoryTypesEnum? SubCategoryType;

            /// <summary>
            /// Only when BuildType is null, true means all, false means none
            /// </summary>
            public bool AllBuildTypes;

            /// <summary>
            /// Only when CategoryType is null, true means all, false means none
            /// </summary>
            public bool AllCategoryTypes;

            /// <summary>
            /// Only when SubCategoryType is null, true means all, false means none
            /// </summary>
            public bool AllSubCategoryTypes;

            public MyPrefabSnapPointType(string prefix, string postfix, Dictionary<string, object> customData)
            {
                string buildTypeKey = prefix + "BUILD_TYPE" + postfix;
                string categoryTypeKey = prefix + "CATEGORY" + postfix;
                string subCategoryTypeKey = prefix + "SUBCATEGORY" + postfix;

                GetEnumValue(GetValue(customData, buildTypeKey), out BuildType, out AllBuildTypes);
                GetEnumValue(GetValue(customData, categoryTypeKey), out CategoryType, out AllCategoryTypes);
                GetEnumValue(GetValue(customData, subCategoryTypeKey), out SubCategoryType, out AllSubCategoryTypes);
            }

            private object GetValue(Dictionary<string, object> customData, string key)
            {
                foreach (var item in customData)
                {
                    if (string.Compare(item.Key, key, true) == 0)
                    {
                        return item.Value;
                    }
                }

                throw new NotImplementedException();
            }

            private void GetEnumValue<T>(object objectValue, out T? enumValue, out bool all) where T : struct
            {
                string stringValue = objectValue == null ? null : objectValue.ToString();
                T value;
                if (!string.IsNullOrEmpty(stringValue) && Enum.TryParse<T>(stringValue, true, out value))
                {
                    enumValue = value;
                    all = false;
                }
                else
                {
                    enumValue = null;
                    all = string.Compare("ANY", stringValue, true) == 0;
                }
            }

            /// <summary>
            /// True, if this snap point can be atached to specified snap point
            /// </summary>
            public bool CanAttachTo(MyPrefabSnapPointType to)
            {
                return
                    (to.AllBuildTypes || to.BuildType == BuildType) &&
                    (to.AllCategoryTypes || to.CategoryType == CategoryType) &&
                    (to.AllSubCategoryTypes || to.SubCategoryType == SubCategoryType);
            }
        }

        /// <summary>
        /// Snap point can't be selected and is not rendered if invisible
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// Name of Snap Point (from dummy dictionary)
        /// </summary>
        public string Name;

        /// <summary>
        /// Snap Point Parent
        /// </summary>
        public MyPrefabBase Prefab { get; set; }

        /// <summary>
        /// Dummy matrix
        /// </summary>
        public Matrix Matrix { get; set; }

        /// <summary>
        /// Type of Snap Point
        /// </summary>
        public MyPrefabSnapPointType SnapType { get; set; }

        /// <summary>
        /// Supported  Snap Points
        /// </summary>
        public List<MyPrefabSnapPointType> SnapTargets { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public MyPrefabSnapPoint(MyPrefabBase prefab)
        {
            Visible = true;
            Prefab = prefab;
            SnapTargets = new List<MyPrefabSnapPointType>();
        }

        /// <summary>
        /// Returns snap point size in screen coordinates (depends on screen reslution)
        /// </summary>
        public static float GetFixedSnapSize()
        {
            return MyGuiManager.GetScreenSizeFromNormalizedSize(new Vector2(0.015f, 0.015f)).Y;
        }

        /// <summary>
        /// Returns snap point size (see GetFixedSnapSize)
        /// </summary>
        public static float GetRealSnapSize()
        {
            return 5.0f;
        }

        /// <summary>
        /// True if this snap point can attach to specified snapPoint
        /// </summary>
        public bool CanAttachTo(MyPrefabSnapPoint snapPoint)
        {
            return Prefab != snapPoint.Prefab && snapPoint.SnapTargets.Exists(a => SnapType.CanAttachTo(a));
        }
    }
}
