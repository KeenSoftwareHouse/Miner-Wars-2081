using System;
using System.Collections.Generic;
using MinerWars.AppCode.Game.Localization;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.AppCode.Game.GUI.ScreenEditor;
using MinerWars.AppCode.Game.Entities.VoxelHandShapes;
using SysUtils.Utils;

namespace MinerWars.AppCode.Game.GUI.Helpers
{
    static class MyGuiEditorVoxelHandHelpers
    {
        static Dictionary<MyVoxelHandShapeType, MyGuiEditorVoxelHandHelper> m_editorVoxelHandShapeHelpers = new Dictionary<MyVoxelHandShapeType, MyGuiEditorVoxelHandHelper>();
        static Dictionary<MyMwcVoxelHandModeTypeEnum, MyGuiEditorVoxelHandHelper> m_editorVoxelHandModeHelpers = new Dictionary<MyMwcVoxelHandModeTypeEnum, MyGuiEditorVoxelHandHelper>();

        //Arrays of enums values
        public static Array MyEditorVoxelHandShapeHelperTypesEnumValues { get; private set; }
        public static Array MyEditorVoxelHandModeHelperTypesEnumValues { get; private set; }

        static MyGuiEditorVoxelHandHelpers()
        {
            MyMwcLog.WriteLine("MyGuiEditorVoxelHandHelpers()");

            MyEditorVoxelHandShapeHelperTypesEnumValues = Enum.GetValues(typeof(MyVoxelHandShapeType));
            MyEditorVoxelHandModeHelperTypesEnumValues = Enum.GetValues(typeof(MyMwcVoxelHandModeTypeEnum));
        }

        public static MyGuiEditorVoxelHandHelper GetEditorVoxelHandShapeHelper(MyVoxelHandShapeType shapeType)
        {
            MyGuiEditorVoxelHandHelper ret;
            if (m_editorVoxelHandShapeHelpers.TryGetValue(shapeType, out ret))
                return ret;
            else
                return null;
        }

        public static MyGuiEditorVoxelHandHelper GetEditorVoxelHandModeHelper(MyMwcVoxelHandModeTypeEnum modeType)
        {
            MyGuiEditorVoxelHandHelper ret;
            if (m_editorVoxelHandModeHelpers.TryGetValue(modeType, out ret))
                return ret;
            else
                return null;
        }

        public static void UnloadContent()
        {

            m_editorVoxelHandShapeHelpers.Clear();
            m_editorVoxelHandModeHelpers.Clear();

        }

        public static void LoadContent()
        {

            #region Editor voxel hand shape helpers

            m_editorVoxelHandShapeHelpers.Add(MyVoxelHandShapeType.Sphere,
                new MyGuiEditorVoxelHandHelper(null, MyTextsWrapperEnum.EditVoxelHandShapeSphere));
            m_editorVoxelHandShapeHelpers.Add(MyVoxelHandShapeType.Box,
                new MyGuiEditorVoxelHandHelper(null, MyTextsWrapperEnum.EditVoxelHandShapeBox));
            m_editorVoxelHandShapeHelpers.Add(MyVoxelHandShapeType.Cuboid,
                new MyGuiEditorVoxelHandHelper(null, MyTextsWrapperEnum.EditVoxelHandShapeCuboid));
            m_editorVoxelHandShapeHelpers.Add(MyVoxelHandShapeType.Cylinder,
                new MyGuiEditorVoxelHandHelper(null, MyTextsWrapperEnum.EditVoxelHandShapeCylinder));

            #endregion

            #region Editor voxel hand mode helpers

            m_editorVoxelHandModeHelpers.Add(MyMwcVoxelHandModeTypeEnum.ADD,
                new MyGuiEditorVoxelHandHelper(null, MyTextsWrapperEnum.AddVoxels));

            m_editorVoxelHandModeHelpers.Add(MyMwcVoxelHandModeTypeEnum.SUBTRACT,
                new MyGuiEditorVoxelHandHelper(null, MyTextsWrapperEnum.RemoveVoxels));

            m_editorVoxelHandModeHelpers.Add(MyMwcVoxelHandModeTypeEnum.SET_MATERIAL,
                new MyGuiEditorVoxelHandHelper(null, MyTextsWrapperEnum.ChangeVoxelMaterial));

            m_editorVoxelHandModeHelpers.Add(MyMwcVoxelHandModeTypeEnum.SOFTEN,
                new MyGuiEditorVoxelHandHelper(null, MyTextsWrapperEnum.SoftenVoxels));

            m_editorVoxelHandModeHelpers.Add(MyMwcVoxelHandModeTypeEnum.WRINKLE,
                new MyGuiEditorVoxelHandHelper(null, MyTextsWrapperEnum.WrinkleVoxels));
            #endregion
        }
    }
}
