﻿using System;
using GXPEngine.Core;
using GXPEngine.Entities;
using GXPEngine.StageManagement;

namespace GXPEngine.Abilities
{
    public class BurgerPunch : BasicMelee
    {
        public BurgerPunch() : base("hitboxes/burger_punch.png", 1, 1)
        {
            xCoordinates = new Vector2(-width, width * 1.5f);
            damage = 2;
            y = -1.5f * height;
            alpha = 50;
            attackDuration = 300;
            coolDown = 1500;
            
            SetSound("sounds/burger_punch.wav",0.5f);
        }
    }

    public class PastaWhip : BasicMelee
    {
        public PastaWhip() : base("hitboxes/pasta_whip.png", 1, 1)
        {
            xCoordinates = new Vector2(-width, 0.3f*width);
            damage = 1;
            y = -2.5f * height;
            alpha = 50;
            attackDuration = 300;
            coolDown = 1000;
            
            SetSound("sounds/pasta_whip.wav",0.5f);
        }
    }

    public class PizzaBite : BasicMelee
    {
        public PizzaBite() : base("hitboxes/pizza_bite.png", 1, 1)
        {
            xCoordinates = new Vector2(-width, width * 1.5f);
            damage = 1;
            y = -1.5f * height;
            alpha = 50;
            
            //random time, so that the enemies aren't attacking all at once (only set once)
            attackDuration = Utils.Random (300, 1000);   
            coolDown = Utils.Random(1000, 2500); 
            
            SetSound("sounds/pizza_bite.wav", 0.05f);
        }
    }

    public class BurgerExplosion : BasicMelee
    {
        public BurgerExplosion() : base("hitboxes/burger_explosion.png", 1, 1)
        {
            xCoordinates = new Vector2(-width*0.5f, -width/2.25f);
            damage = 2;
            y =  -0.75f*height;
            alpha = 50;

            attackDuration = 500;
            coolDown = 500;
            
            SetSound("sounds/burger_explosion.wav", 0.5f);
        }
    }

    public class SeedShooter : Ability
    {
        private float speed;
        public SeedShooter() : base("hitboxes/basically_nothing.png",1,1)
        {
            damage = 1;
            speed = 0.7f;
            xCoordinates = new Vector2(0, -width);
            y = 0;
            coolDown = 500;

            SetSound("sounds/seed_shooter.wav", 0.5f);
        }

        protected override void Action()
        {
            Entity parent = (Entity) this.parent;
            Vector2 playerPos = InverseTransformPoint(myGame.player.x, myGame.player.y);
            Vector2 parentPos = InverseTransformPoint(parent.x,parent.y);

            Vector2 direction = playerPos - parentPos;
            
            direction.Normalize();
            
            

            Seed seed = new Seed(direction,speed,damage, (Entity) parent);
     
            
            
            seed.SetXY(parent.x,parent.y - 50);

            StageLoader.AddObject(seed);
            Console.WriteLine(parent.x);
            
            

        }
    }   

    public class MeatballShooter : Ability
    {
        private float speed;
        public MeatballShooter() : base("hitboxes/basically_nothing.png",1,1)
        {
            damage = 2;
            speed = 1.0f;
            xCoordinates = new Vector2(0, width-10);
            y = -0.7f * height;
            coolDown = 500;
         
        }
        
        protected override void Action()
        {
            Entity parent = (Entity) this.parent;
            Vector2 direction = new Vector2(parent.mirrored ? -1 : 1, 0);

            Meatball meatball = new Meatball(direction,speed,damage, parent);                      
                                                                                                 
            StageLoader.AddObject(meatball);      
            meatball.SetXY(parent.x,parent.y - 120);
        }
    }
}