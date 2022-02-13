﻿
using System;
using System.Threading;
using GXPEngine.Core;

namespace GXPEngine
{
    /// <summary>
    /// Class that covers all types of entities (players and enemies)
    /// </summary>
    public class Entity : Sprite
    {
        public Weapon weapon;

        //STATS
        public float health;
        public float damage;

        //Invincibility duration after being hit
        private int damageTime;
        protected int invincibilityDuration;
        private bool damageable;


        public bool FeetHitBoxIsVisible = false;
        
        protected Vector2 velocity;
        protected float speed;
        protected State currentState;
        
        public AnimationSprite model;  //changed to protected to test it out in tempEnemy
        protected EasyDraw canvas;      //changed to protected to test it out in tempEnemy

        public Sprite bodyHitbox;

        public String tag;
        


        /// <summary>
        /// All enemies and players are entities, all entities can move, are animated and have hitboxes.
        /// The sprite of this object functions as the hitbox for the feet of the entity, this hitbox is thus
        /// mostly used for walking.
        /// </summary>
        /// <param name="hitboxPath">Filepath of the image used for the hitbox of the feet of the entity</param>
        /// <param name="modelPath">Filepath of the image used for the entity's model</param>
        /// <param name="columns">The amount of columns the spritesheet has</param>
        /// <param name="rows">The amount of rows the spritesheet has</param>
        protected Entity(string hitboxPath, string modelPath, int columns, int rows) : base(hitboxPath)  //feet HitBox are the base for animationSprite, that means that the base needs to be in the correct size so that the animationsprite fits
        {
            damageable = true;
            
            //Model of the entity
            model = new AnimationSprite(modelPath, columns, rows, addCollider:false);
            model.SetXY(x-32,-model.height+height);                               
            AddChild(model);

            //Canvas for debug purposes
            canvas = new EasyDraw(width, height, false);             
            AddChildAt(canvas,0);
        }

        /// <summary>
        /// Sets the weapon of the entity
        /// </summary>
        public void SetWeapon(Weapon newWeapon)
        {
            weapon = newWeapon;
            AddChild(weapon);
        }
        
        /// <summary>
        /// Sets the hitbox of the body
        /// </summary>
        /// <param name="path">The filepath for the image used</param>
        /// <param name="x">The x coordinate of the location of the hitbox</param>
        /// <param name="y">The y coordinate of the location of the hitbox</param>
        protected void SetBodyHitbox(string path, float x, float y)
        {
            bodyHitbox = new Sprite(path);
            bodyHitbox.alpha = 150;
            bodyHitbox.collider.isTrigger = true;
            bodyHitbox.x = x;
            bodyHitbox.y = y;
            AddChild(bodyHitbox);
        }
        
        /// <summary>
        /// Damages the entity for a certain amount of damage. 
        /// </summary>
        public void Damage(float amount)
        {
            if (damageable)
            {
                health -= amount;
            
                if (health <= 0)
                {
                    Kill();  
                }

                damageTime = Time.now;
                damageable = false;
                
                model.SetColor(0,255,0);
                Console.WriteLine(model.name + ", " + "Health: " + health);

            }
        }

        /// <summary>
        /// Adds a given amount of health to the health pool of the entity.
        /// </summary>
        public void AddHealth(float amount)
        {
            health += amount;
        }
        
        /// <summary>
        /// Kills the entity
        /// </summary>
        public void Kill()
        {
            this.LateDestroy();
        }

        /// <summary>
        /// Every frame the entity's movement, animation and state are updated.
        /// When overriding this always call this base at the end by using base.Update();
        /// </summary>
        protected virtual void Update()
        {
            model.Animate(Time.deltaTime);
            UpdateState();

            //Updates movement and fixes mirror
            if (velocity != new Vector2(0, 0)) UpdateMovement();
            
            
            //Debugging
            if (debugMode)
            {
                canvas.Fill(255,0,0);
                canvas.ShapeAlign(CenterMode.Min,CenterMode.Min);
                canvas.Rect(0,0,width,height);
            }
            else
            {
                canvas.ClearTransparent();
            }

            //Invincibility frames
            if (Time.now - damageTime > invincibilityDuration)
            {
                damageable = true;
                model.SetColor(255, 0, 0);
            }

            if (!damageable)
            {
                model.alpha = Utils.Random(60, 100);
            }
        }



        /// <summary>
        /// Sets the delay between animation frames, can be set individually for entities to ensure nice animations
        /// </summary>
        /// <param name="delay">The amount of delay between animation frames, can range from 0-255</param>
        protected void SetAnimationDelay(byte delay)
        {
            model.SetCycle(1,model.frameCount,delay);
        }

        /// <summary>
        /// Updates the entities movement based on its speed and Time.deltaTime
        /// </summary>
        protected virtual void UpdateMovement()
        {
            //seperate MoveUntilCollision into x and y so that the player doesn't get stuck in a wall when moving diagonally (Game Programming Recording 2: 1:16:56)
            MoveUntilCollision(0, velocity.y * Time.deltaTime * speed);
            MoveUntilCollision(velocity.x * Time.deltaTime * speed, 0);
            
            //Fixes mirroring based on the velocity
            bool mirror = velocity.x < 0;

            Mirror(mirror,false);
            model.Mirror(mirror,false);

            if (weapon != null)
            {
                weapon.Mirror(mirror,false);
            }
            
            
            //Reset the velocity
            velocity.Set(0,0);
        }

        /// <summary>
        /// Enum that determines in what state the entity is
        /// </summary>
        protected enum State
        {
            Walk,
            Stand,
            Jump
        }
        
        /// <summary>
        /// Updates the currentState
        /// </summary>
        private void UpdateState()
        {
            if (velocity.Magnitude() == 0)
            {
                currentState = State.Stand;
                UpdateAnimation();
            }
            else if (currentState != State.Walk)
            {
                currentState = State.Walk;
                UpdateAnimation();
            }
        }
        
        /// <summary>
        /// Updates the animationcycle based on the currentState
        /// </summary>
        private void UpdateAnimation()
        {
            switch (currentState)
            {
                case State.Stand:
                    model.SetCycle(5,3);
                    break;
                case State.Walk:
                    model.SetCycle(1,3);
                    break;
                case State.Jump:
                    model.SetCycle(4,1);
                    break;
            }
        }
    }
}