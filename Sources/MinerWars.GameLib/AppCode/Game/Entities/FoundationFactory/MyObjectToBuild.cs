using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWars.AppCode.Game.Gameplay;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.SubObjects;

namespace MinerWars.AppCode.Game.Entities.FoundationFactory
{
    /// <summary>
    /// Object, which can be build in foundation factory
    /// </summary>
    class MyObjectToBuild
    {
        /// <summary>
        /// Object's object builder
        /// </summary>
        public MyMwcObjectBuilder_Base ObjectBuilder { get; set; }

        /// <summary>
        /// Building specification
        /// </summary>
        public MyBuildingSpecification BuildingSpecification { get; set; }

        /// <summary>
        /// Amount of builded object
        /// </summary>
        public float Amount { get; set; }
        
        /// <summary>
        /// Creates new instance of object to build
        /// </summary>
        /// <param name="objectBuilder">Object builder</param>
        /// <param name="buildingSpecification">Building specification</param>
        /// <param name="amount">Amount</param>
        public MyObjectToBuild(MyMwcObjectBuilder_Base objectBuilder, MyBuildingSpecification buildingSpecification, float amount)
        {
            ObjectBuilder = objectBuilder;
            BuildingSpecification = buildingSpecification;
            Amount = amount;
        }                

        /// <summary>
        /// Returns new instance of object to build with max amount from gameplay properties
        /// </summary>
        /// <param name="objectBuilder">Object builder</param>
        /// <returns>New instance of object to build</returns>
        public static MyObjectToBuild CreateFromObjectBuilder(MyMwcObjectBuilder_Base objectBuilder)
        {
            //warning: use default faction for get gameplay properties for creating object to build
            float amount = MyGameplayConstants.GetGameplayProperties(objectBuilder, MyMwcObjectBuilder_FactionEnum.Euroamerican).MaxAmount;
            return MyObjectToBuild.CreateFromObjectBuilder(objectBuilder, amount);
        }

        /// <summary>
        /// Returns new instance of object to build with specified amount
        /// </summary>
        /// <param name="objectBuilder">Object builder</param>
        /// <param name="amount">Amount</param>
        /// <returns>New instance of object to build</returns>
        public static MyObjectToBuild CreateFromObjectBuilder(MyMwcObjectBuilder_Base objectBuilder, float amount)
        {
            MyBuildingSpecification buildingSpecification = MyBuildingSpecifications.GetBuildingSpecification(objectBuilder);
            MyObjectToBuild objectToBuild = new MyObjectToBuild(objectBuilder, buildingSpecification, amount);
            return objectToBuild;
        }

        public static bool HasSameObjectBuilders(MyObjectToBuild obj1, MyObjectToBuild obj2)
        {
            if (obj1 == null || obj2 == null)
            {
                return false;
            }
            else
            {
                return obj1.ObjectBuilder.GetObjectBuilderType() == obj2.ObjectBuilder.GetObjectBuilderType() &&
                       obj1.ObjectBuilder.GetObjectBuilderId() == obj2.ObjectBuilder.GetObjectBuilderId();
            }
        }

        public bool IsSame(MyObjectToBuild other)
        {
            if (other == null)
            {
                return false;
            }

            if(Amount != other.Amount)
            {
                return false;
            }

            bool hasSameObjectBuilder = ObjectBuilder.GetObjectBuilderType() == other.ObjectBuilder.GetObjectBuilderType() &&
                       ObjectBuilder.GetObjectBuilderId() == other.ObjectBuilder.GetObjectBuilderId();

            if(ObjectBuilder is MyMwcObjectBuilder_PrefabBase)
            {
                hasSameObjectBuilder = hasSameObjectBuilder &&
                                       ((MyMwcObjectBuilder_PrefabBase) ObjectBuilder).FactionAppearance ==
                                       ((MyMwcObjectBuilder_PrefabBase) other.ObjectBuilder).FactionAppearance;
            }

            return hasSameObjectBuilder;
        }
    }
}
