using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;

namespace hyppypeli;

public class hyppypeli : PhysicsGame
{
    private const double NOPEUS = 200;
    private const double HYPPYNOPEUS = 750;
    private const int RUUDUN_KOKO = 40;

    private PlatformCharacter pelaaja1;

    private Image pelaajanKuva;
    private Image morkokuva = LoadImage("mörkö.png");
    private Image tahtiKuva = LoadImage("tahti.png");
    private Image estekuva = LoadImage("piikki.png");

    private SoundEffect maaliAani = LoadSoundEffect("maali.wav");

    public hyppypeli()
    {
        pelaajanKuva = LoadImage("tyyppi.png");
    }

    public override void Begin()
    {
        Gravity = new Vector(0, -1000);

        LuoKentta();
        LisaaNappaimet();

        Camera.Follow(pelaaja1);
        Camera.ZoomFactor = 1.2;
        Camera.StayInLevel = true;

        MasterVolume = 0.5;



    }

    void lisaamörkö(Vector paikka, double leveys, double korkeus) 
    {PhysicsObject morko = new PhysicsObject(40, 20);
        morko.Image = morkokuva;
    Add(morko);
    morko.Tag = "morko";
    morko.Position = paikka; 
    FollowerBrain seuraajanAivot = new FollowerBrain(pelaaja1);
    seuraajanAivot.Speed = 150;
    morko.Brain = seuraajanAivot;
    }

void törmäävät (PhysicsObject tormaaja, PhysicsObject kohde)
{
    tormaaja.Destroy(); 
    kuolemaaani.Play();
    ClearAll();
    LuoKentta(); 
    LisaaNappaimet();
    Camera.Follow(pelaaja1);
}
    private void LuoKentta()
    {
        TileMap kentta = TileMap.FromLevelAsset("kentta1.txt");
        kentta.SetTileMethod('#', LisaaTaso);
        kentta.SetTileMethod('*', LisaaTahti);
        kentta.SetTileMethod('N', LisaaPelaaja);
        kentta.SetTileMethod('e', LisaaEste);
        kentta.SetTileMethod('M',lisaamörkö );
        kentta.Execute(RUUDUN_KOKO, RUUDUN_KOKO);
        Level.CreateBorders();
        Level.Background.CreateGradient(Color.White, Color.SkyBlue);

    }

    private void LisaaEste(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject este = PhysicsObject.CreateStaticObject(leveys, korkeus);
        este.Image = estekuva;
        este.Position = paikka;
        AddCollisionHandler(este, TormasiEsteeseen);

        Add(este);
    }
   
    


private void TormasiEsteeseen(PhysicsObject tormaaja, PhysicsObject kohde)
    {
        kohde.Destroy();
        kuolemaaani.Play();
        ClearAll();
        LuoKentta(); 
        LisaaNappaimet();
        Camera.Follow(pelaaja1);
    }
    SoundEffect kuolemaaani = LoadSoundEffect("super-mario-death-sound-sound-effect.wav");
    private void LisaaTaso(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject taso = PhysicsObject.CreateStaticObject(leveys, korkeus);
        taso.Position = paikka;
        taso.Color = Color.Green;
        Add(taso);
    }

    private void LisaaTahti(Vector paikka, double leveys, double korkeus)
    {
        PhysicsObject tahti = PhysicsObject.CreateStaticObject(leveys, korkeus);
        tahti.IgnoresCollisionResponse = true;
        tahti.Position = paikka;
        tahti.Image = tahtiKuva;
        tahti.Tag = "tahti";
        Add(tahti);
    }

    private void LisaaPelaaja(Vector paikka, double leveys, double korkeus)
    {
        pelaaja1 = new PlatformCharacter(leveys, korkeus)
        {
            Position = paikka,
            Mass = 4.0,
            Image = pelaajanKuva
        };
        AddCollisionHandler(pelaaja1, "tahti", TormaaTahteen);
        Add(pelaaja1);
        AddCollisionHandler(pelaaja1, "morko" ,törmäävät);
    }

    private void LisaaNappaimet()
    {
        Keyboard.Listen(Key.F1, ButtonState.Pressed, ShowControlHelp, "Näytä ohjeet");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");

        Keyboard.Listen(Key.Left, ButtonState.Down, Liikuta, "Liikkuu vasemmalle", pelaaja1, -NOPEUS);
        Keyboard.Listen(Key.Right, ButtonState.Down, Liikuta, "Liikkuu vasemmalle", pelaaja1, NOPEUS);
        Keyboard.Listen(Key.Up, ButtonState.Pressed, Hyppaa, "Pelaaja hyppää", pelaaja1, HYPPYNOPEUS);

        ControllerOne.Listen(Button.Back, ButtonState.Pressed, Exit, "Poistu pelistä");

        ControllerOne.Listen(Button.DPadLeft, ButtonState.Down, Liikuta, "Pelaaja liikkuu vasemmalle", pelaaja1,
            -NOPEUS);
        ControllerOne.Listen(Button.DPadRight, ButtonState.Down, Liikuta, "Pelaaja liikkuu oikealle", pelaaja1, NOPEUS);
        ControllerOne.Listen(Button.A, ButtonState.Pressed, Hyppaa, "Pelaaja hyppää", pelaaja1, HYPPYNOPEUS);

        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
    }

    private void Liikuta(PlatformCharacter hahmo, double nopeus)
    {
        hahmo.Walk(nopeus);
    }

    private void Hyppaa(PlatformCharacter hahmo, double nopeus)
    {
        hahmo.Jump(nopeus);
    }

    private void TormaaTahteen(PhysicsObject hahmo, PhysicsObject tahti)
    {
        maaliAani.Play();
        MessageDisplay.Add("Keräsit tähden!");
        tahti.Destroy();
    }
}