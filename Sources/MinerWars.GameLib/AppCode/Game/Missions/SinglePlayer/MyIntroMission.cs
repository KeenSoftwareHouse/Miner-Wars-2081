using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MinerWarsMath;
using MinerWars.AppCode.Game.Inventory;
using MinerWars.AppCode.Game.Managers.Session;
using MinerWars.CommonLIB.AppCode.ObjectBuilders;
using MinerWars.AppCode.Game.Localization;

namespace MinerWars.AppCode.Game.Missions.SinglePlayer
{
    class MyIntroMission : MyMission
    {
        public MyIntroMission()
        {
            ID = MyMissionID.M01_Intro;
            DebugName = new StringBuilder("Intro mission");
            Name = MyTextsWrapperEnum.M01_Intro;
            Description = MyTextsWrapperEnum.M01_Intro_Description;

            //Location = new Vector3(100, -300, 0);
            RequiredMissions = new MyMissionID[] { };
            //RequiredTime = new DateTime(1, 1, 1, 0, 0, 15);

            // use MyFirstMission
            //m_submissions = new List<MySubmission>
            //                    {
            //                        new MySubmission(
            //                                ID: MyMissionID.M01_Intro_Mining,
            //                                Name: new StringBuilder("Mine 50 ore"),
            //                                Description: new StringBuilder(""),
            //                                Icon: null,
            //                                ParentMission: this,
            //                                RequiredMissions: new MyMissionID[] {},
            //                                Location: new Vector3(250, 250, 250)
            //                            ),

            //                        new MySubmission(
            //                                ID: MyMissionID.M01_Intro_Sell,
            //                                Name: new StringBuilder("Sell the ore"),
            //                                Description: new StringBuilder(""),
            //                                Icon: null,
            //                                ParentMission: this,
            //                                RequiredMissions: new MyMissionID[] { MyMissionID.M01_Intro_Mining },
            //                                Location: new Vector3(100, 500, 500)
            //                            ),

            //                        new MySubmission(
            //                                ID: MyMissionID.M01_Intro_Buy,
            //                                Name: new StringBuilder("Buy a weapon"),
            //                                Description: new StringBuilder(""),
            //                                Icon: null,
            //                                ParentMission: this,
            //                                RequiredMissions: new MyMissionID[] { MyMissionID.M01_Intro_Sell },
            //                                Location: new Vector3(500, 100, 500)
            //                            ),

            //                        new MySubmission(
            //                                ID: MyMissionID.M01_Intro_Kill,
            //                                Name: new StringBuilder("Kill the enemy"),
            //                                Description: new StringBuilder("He may not survive."),
            //                                Icon: null,
            //                                ParentMission: this,
            //                                RequiredMissions: new MyMissionID[] { MyMissionID.M01_Intro_Buy },
            //                                Location: new Vector3(500, 500, 100)
            //                            )
            //                    };

            //MyMissions.AddMission(new MySubmission(
            //        ID: MyMissionID.M01_Intro_Mining,
            //        Name: new StringBuilder("Mine"),
            //        Description: new StringBuilder(""),
            //        Icon: null,
            //        ParentMission: this,
            //        RequiredMissions: new MyMissionID[] { MyMissionID.M01_Intro },
            //        Location: new Vector3(500, 500, 500)
            //        ));

            //MyMissions.AddMission(new MySubmission(
            //        ID: MyMissionID.M01_Intro_Sell,
            //        Name: new StringBuilder("Sell the ore"),
            //        Description: new StringBuilder(""),
            //        Icon: null,
            //        ParentMission: this,
            //        RequiredMissions: new MyMissionID[] { MyMissionID.M01_Intro_Mining },
            //        Location: new Vector3(100, 500, 500)
            //        ));

            //MyMissions.AddMission(new MySubmission(
            //        ID: MyMissionID.M01_Intro_Buy,
            //        Name: new StringBuilder("Buy a weapon"),
            //        Description: new StringBuilder(""),
            //        Icon: null,
            //        ParentMission: this,
            //        RequiredMissions: new MyMissionID[] { MyMissionID.M01_Intro_Sell },
            //        Location: new Vector3(500, 100, 500)
            //        ));

            //MyMissions.AddMission(new MySubmission(
            //        ID: MyMissionID.M01_Intro_Kill,
            //        Name: new StringBuilder("Kill the enemy"),
            //        Description: new StringBuilder("He may not survive."),
            //        Icon: null,
            //        ParentMission: this,
            //        RequiredMissions: new MyMissionID[] { MyMissionID.M01_Intro_Buy },
            //        Location: new Vector3(500, 500, 100)
            //        ));
        }

        public override void Load()
        {
            base.Load();

            MySession.PlayerShip.Inventory.OnInventoryItemAmountChange += InventoryContentChanged;
        }

        private void InventoryContentChanged(MyInventory sender, MyInventoryItem inventoryItem, float amountChanged)
        {                        
            float oreAmount = sender.GetInventoryItemsCount(MyMwcObjectBuilderTypeEnum.Ore, null);
            if (oreAmount >= 50)
            {
                m_objectives.Find(submission => submission.ID == MyMissionID.M01_Intro_Mining).Success();
            }
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Success()
        {
            base.Success();

            MySession.PlayerShip.Inventory.OnInventoryItemAmountChange -= InventoryContentChanged;
        }
    }
}
