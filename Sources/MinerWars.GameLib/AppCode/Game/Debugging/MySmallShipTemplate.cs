using System.Text;
using MinerWars.CommonLIB.AppCode.ObjectBuilders.Object3D;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;

namespace MinerWars.AppCode.Game.Managers.Others
{
    class MySmallShipTemplate
    {
        public int ID { get; set; }

        public StringBuilder Name { get; set; }

        public MyMwcObjectBuilder_SmallShip Builder { get; set; }

        public bool SavedToServer { get; set; }

        public MySmallShipTemplate(int id, StringBuilder name, MyMwcObjectBuilder_SmallShip builder, bool isSavedOnServer)
        {
            ID = id;
            Name = name;
            Builder = builder;
            SavedToServer = isSavedOnServer;
        }

        public MySmallShipTemplate(MyMwcObjectBuilder_SmallShipTemplate template)
            : this(template.ID, new StringBuilder(template.Name), template.Builder, true) 
        {

        }

        public MyMwcObjectBuilder_SmallShipTemplate GetObjectBuilder() 
        {
            return new MyMwcObjectBuilder_SmallShipTemplate(ID, Name.ToString(), Builder);
        }

        public void ApplyToSmallShipBuilder(MyMwcObjectBuilder_SmallShip builder) 
        {
            var cloneBuilder = Builder.Clone() as MyMwcObjectBuilder_SmallShip;
            builder.Inventory = cloneBuilder.Inventory;
            builder.Weapons = cloneBuilder.Weapons;
            builder.Armor = cloneBuilder.Armor;
            builder.Engine = cloneBuilder.Engine;
            builder.Radar = cloneBuilder.Radar;
            builder.ShipType = cloneBuilder.ShipType;
        }
    }
}