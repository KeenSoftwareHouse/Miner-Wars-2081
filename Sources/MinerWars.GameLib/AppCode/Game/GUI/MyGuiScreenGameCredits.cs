using System.Collections.Generic;
using System.Text;
using MinerWarsMath;
using MinerWarsMath.Graphics;
using MinerWars.AppCode.Toolkit.Input;
using MinerWars.AppCode.App;
using MinerWars.AppCode.Game.Audio;
using MinerWars.AppCode.Game.GUI.Core;
using MinerWars.AppCode.Game.Managers;
using MinerWars.AppCode.Game.Utils;
using MinerWars.CommonLIB.AppCode.Networking;
using MinerWars.CommonLIB.AppCode.Utils;
using MinerWars.AppCode.Game.Textures;

//  This class is called from gameplay screen a is only for drawing scrolling credits.

//  IMPORTANT: Don't forget that here we are using texts that didn't go through text resources so if some credits text contains
//  character that isn't in our font, it will fail. Therefore, if you use something obscure (not in ASCII), add it to font as a special character - AND TEST IT!!!


namespace MinerWars.AppCode.Game.GUI
{
    class MyGuiScreenGameCredits : MyGuiScreenBase
    {
        class MyCreditsDepartment
        {
            public StringBuilder Name;
            public List<MyCreditsPerson> Persons;

            public MyCreditsDepartment(string name)
            {
                Name = new StringBuilder(name);
                Persons = new List<MyCreditsPerson>(5);
            }
        }

        class MyCreditsPerson
        {
            public StringBuilder Name;

            public MyCreditsPerson(string name)
            {
                Name = new StringBuilder(name);
            }
        }

        
        Color color = new Color(255, 255, 255, 220); //  Red 
        const float NUMBER_OF_SECONDS_TO_SCROLL_THROUGH_WHOLE_SCREEN = 30;
        List<MyCreditsDepartment> m_departments;
        float m_scrollingPositionY;
        MyTexture2D m_keenswhLogoTexture, m_menuOverlay;
        float m_startTimeInMilliseconds;

        public MyGuiScreenGameCredits()
            : base(Vector2.Zero, null, null)
        {
            m_startTimeInMilliseconds = MyMinerGame.TotalGamePlayTimeInMilliseconds;
        }

        public override string GetFriendlyName()
        {
            return "MyGuiScreenGameCredits";
        }

        public override void LoadContent()
        {
            DrawMouseCursor = false;
            m_closeOnEsc = true;

            m_keenswhLogoTexture =
                MyTextureManager.GetTexture<MyTexture2D>(
                    "Textures\\GUI\\MinerWarsLogoLarge", flags: TextureFlags.IgnoreQuality);

            m_menuOverlay = MyTextureManager.GetTexture<MyTexture2D>("Textures\\GUI\\BackgroundScreen\\MainMenuOverlay", flags: TextureFlags.IgnoreQuality);

            //  We will start scrolling from the bottom
            ResetScrollingPositionY(-0.2f);

            //  List of departments
            m_departments = new List<MyCreditsDepartment>();

            //  Director
            MyCreditsDepartment director = new MyCreditsDepartment("Produced and Directed By");
            m_departments.Add(director);
            director.Persons = new List<MyCreditsPerson>();
            director.Persons.Add(new MyCreditsPerson("MAREK ROSA"));


            //  Lead Programmer
            MyCreditsDepartment leadProgrammer = new MyCreditsDepartment("Lead Programmer");
            m_departments.Add(leadProgrammer);
            leadProgrammer.Persons = new List<MyCreditsPerson>();
            leadProgrammer.Persons.Add(new MyCreditsPerson("PETR MINARIK"));

            //  Programmers
            MyCreditsDepartment programmers = new MyCreditsDepartment("Programmers");
            m_departments.Add(programmers);
            programmers.Persons = new List<MyCreditsPerson>();
            programmers.Persons.Add(new MyCreditsPerson("ONDREJ PETRZILKA"));
            programmers.Persons.Add(new MyCreditsPerson("PETR KOLDA"));
            programmers.Persons.Add(new MyCreditsPerson("MARTIN BAUER"));
            programmers.Persons.Add(new MyCreditsPerson("SIMON SOTAK"));
            programmers.Persons.Add(new MyCreditsPerson("JAN KADLEC"));
            programmers.Persons.Add(new MyCreditsPerson("FILIP BUCHTA"));
            programmers.Persons.Add(new MyCreditsPerson("MARTIN VAVREK"));
            programmers.Persons.Add(new MyCreditsPerson("JAKUB DRAZKA"));
            programmers.Persons.Add(new MyCreditsPerson("MICHAL STEFAN"));


            //  Graphic artists 3d
            MyCreditsDepartment artists3d = new MyCreditsDepartment("3D Artists");
            m_departments.Add(artists3d);
            artists3d.Persons = new List<MyCreditsPerson>();
            artists3d.Persons.Add(new MyCreditsPerson("JAN ABSOLIN"));
            artists3d.Persons.Add(new MyCreditsPerson("FILIP NOVY"));
            artists3d.Persons.Add(new MyCreditsPerson("IVAN CHEREVKO"));
            artists3d.Persons.Add(new MyCreditsPerson("JAN KUDRNAC"));
            artists3d.Persons.Add(new MyCreditsPerson("SLOBODAN STEVIC"));
            artists3d.Persons.Add(new MyCreditsPerson("RASTKO STANOJEVIC"));
            artists3d.Persons.Add(new MyCreditsPerson("ADAM M SMITH"));


            //  Graphic artists 2d
            MyCreditsDepartment artists2d = new MyCreditsDepartment("2D Artists");
            m_departments.Add(artists2d);
            artists2d.Persons = new List<MyCreditsPerson>();
            artists2d.Persons.Add(new MyCreditsPerson("JAN CISTIN"));


            //  Writers
            MyCreditsDepartment writers = new MyCreditsDepartment("Writers");
            m_departments.Add(writers);
            writers.Persons = new List<MyCreditsPerson>();
            writers.Persons.Add(new MyCreditsPerson("JAN JIRKOVSKY"));


            //Level designers
            MyCreditsDepartment levelDesigners = new MyCreditsDepartment("Level Designers");
            m_departments.Add(levelDesigners);
            levelDesigners.Persons = new List<MyCreditsPerson>();
            levelDesigners.Persons.Add(new MyCreditsPerson("TOMAS RAMPAS"));
            levelDesigners.Persons.Add(new MyCreditsPerson("MICHAL ROCH"));
            levelDesigners.Persons.Add(new MyCreditsPerson("PETER NESPESNY"));
            levelDesigners.Persons.Add(new MyCreditsPerson("PROKOP SMETANA"));
            levelDesigners.Persons.Add(new MyCreditsPerson("JAKUB GUMAN"));
            levelDesigners.Persons.Add(new MyCreditsPerson("ADAM SKVOR"));
            levelDesigners.Persons.Add(new MyCreditsPerson("MARTIN VANO"));


            //  Sound Designers
            MyCreditsDepartment soundEffects = new MyCreditsDepartment("Sound Designers");
            m_departments.Add(soundEffects);
            soundEffects.Persons = new List<MyCreditsPerson>();
            soundEffects.Persons.Add(new MyCreditsPerson("LUKAS TVRDON"));
            soundEffects.Persons.Add(new MyCreditsPerson("DAN WENTZ"));


            //  Composers
            MyCreditsDepartment composers = new MyCreditsDepartment("Music Composers");
            m_departments.Add(composers);
            composers.Persons = new List<MyCreditsPerson>();
            composers.Persons.Add(new MyCreditsPerson("KAREL ANTONIN"));
            composers.Persons.Add(new MyCreditsPerson("MAREK MRKVICKA"));


            //  Community & PR Managers
            MyCreditsDepartment managers = new MyCreditsDepartment("Community & PR Managers");
            m_departments.Add(managers);
            managers.Persons = new List<MyCreditsPerson>();
            managers.Persons.Add(new MyCreditsPerson("JAN JIRKOVSKY"));
            managers.Persons.Add(new MyCreditsPerson("ANSEL LEOS"));
            managers.Persons.Add(new MyCreditsPerson("NICK MILLER"));


            //  Testers
            MyCreditsDepartment testers = new MyCreditsDepartment("Testers");
            m_departments.Add(testers);
            testers.Persons = new List<MyCreditsPerson>();
            testers.Persons.Add(new MyCreditsPerson("MICHAL LISKA"));
            testers.Persons.Add(new MyCreditsPerson("JAN BASTL"));


            //  Voice Talent
            MyCreditsDepartment voiceTalent = new MyCreditsDepartment("Voice Actors");
            m_departments.Add(voiceTalent);
            voiceTalent.Persons = new List<MyCreditsPerson>();
            voiceTalent.Persons.Add(new MyCreditsPerson("JOHN MCCALMONT"));
            voiceTalent.Persons.Add(new MyCreditsPerson("TORI KAMAL"));
            voiceTalent.Persons.Add(new MyCreditsPerson("JJ AARONSON"));
            voiceTalent.Persons.Add(new MyCreditsPerson("ROBIN EGERTON"));            
            voiceTalent.Persons.Add(new MyCreditsPerson("AMBER BEARD"));
            voiceTalent.Persons.Add(new MyCreditsPerson("LINDA LAKE"));
            voiceTalent.Persons.Add(new MyCreditsPerson("PETER BAKER"));
            voiceTalent.Persons.Add(new MyCreditsPerson("VICTORIA SCOTT"));
            voiceTalent.Persons.Add(new MyCreditsPerson("MARC CHOLETTE"));
            voiceTalent.Persons.Add(new MyCreditsPerson("DARRIN REVITY"));
            voiceTalent.Persons.Add(new MyCreditsPerson("MIKE CLARKE"));
            voiceTalent.Persons.Add(new MyCreditsPerson("OVAIS MALIK"));
            voiceTalent.Persons.Add(new MyCreditsPerson("JOHN KUBIN"));
            voiceTalent.Persons.Add(new MyCreditsPerson("ALEX RAIN"));
            voiceTalent.Persons.Add(new MyCreditsPerson("GERARD NEIL"));

            //  Special thanks
            MyCreditsDepartment specialThanks = new MyCreditsDepartment("Special Thanks to");
            m_departments.Add(specialThanks);
            specialThanks.Persons = new List<MyCreditsPerson>();
            specialThanks.Persons.Add(new MyCreditsPerson("Our community for supporting us and making this happen."));
            specialThanks.Persons.Add(new MyCreditsPerson(""));
            specialThanks.Persons.Add(new MyCreditsPerson("Our contributors:"));
            specialThanks.Persons.Add(new MyCreditsPerson("MrSanta, WolfDK1984, ratsmt, Shadowkeeper, Gundam288, Steve, Focal,"));
            specialThanks.Persons.Add(new MyCreditsPerson("Scuderia, Kilroy, rawrkitteh, Pyrokinesis1019, TakeiNaodar, Orkpower"));


            //  Final
            MyCreditsDepartment final = new MyCreditsDepartment("For more info see");
            m_departments.Add(final);
            final.Persons = new List<MyCreditsPerson>();
            final.Persons.Add(new MyCreditsPerson("http://www.minerwars.com/Team.aspx"));

            //  IMPORTANT: Base load content must be called after child's load content
            base.LoadContent();
        }

        void ResetScrollingPositionY(float offset = 0f)
        {
            //  We will start scrolling from the bottom
            m_scrollingPositionY = 0.99f + offset;
        }


        public override bool Update(bool hasFocus)
        {
            if (base.Update(hasFocus) == false) return false;

            m_scrollingPositionY -= 1.0f / (NUMBER_OF_SECONDS_TO_SCROLL_THROUGH_WHOLE_SCREEN * MyConstants.PHYSICS_STEPS_PER_SECOND);

            return true;
        }

        Color ChangeTextAlpha(Color origColor, float coordY)
        {
            float fadeEnd = 0.25f;
            float fadeStart = 0.3f;
            float alpha = MathHelper.Clamp((coordY - fadeEnd) / (fadeStart - fadeEnd), 0, 1);

            Color newColor = origColor;
            newColor.A = (byte)(origColor.A * alpha);

            return newColor;
        }

        public Vector2 GetScreenLeftTopPosition()
        {
            float deltaPixels = 25 * MyGuiManager.GetSafeScreenScale();
            Rectangle fullscreenRectangle = MyGuiManager.GetSafeFullscreenRectangle();
            return MyGuiManager.GetNormalizedCoordinateFromScreenCoordinate_FULLSCREEN(new Vector2(deltaPixels, deltaPixels));
        }
        
        public override bool Draw(float backgroundFadeAlpha)
        {
            if (base.Draw(backgroundFadeAlpha) == false) return false;

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //  Credits
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            float movingY = m_scrollingPositionY;

            for (int i = 0; i < m_departments.Count; i++)
            {
                MyGuiManager.DrawString(MyGuiManager.GetFontMinerWarsBlue(), m_departments[i].Name,
                                        new Vector2(0.5f, movingY), 0.78f,
                                        ChangeTextAlpha(color, movingY),
                                        MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);
                movingY += 0.05f;

                for (int j = 0; j < m_departments[i].Persons.Count; j++)
                {
                    MyGuiManager.DrawString(MyGuiManager.GetFontMinerWarsBlue(), m_departments[i].Persons[j].Name,
                                            new Vector2(0.5f, movingY), 1.04f,
                                            ChangeTextAlpha(color, movingY),
                                            MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_CENTER);
                    movingY += 0.05f;
                }

                movingY += 0.04f;
            }

            //  This will start scrolling again after last word was scrolled through the top
            if (movingY <= 0) ResetScrollingPositionY();


            MyGuiManager.DrawSpriteBatch(m_menuOverlay, new Rectangle(0, 0, MyMinerGame.ScreenSize.X, MyMinerGame.ScreenSize.Y), Color.White);

            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //  Company Logo - with shadow (thus drawing two times)
            ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            DrawMinerWarsLogo();

            return true;
        }
    }
}