﻿using System;
using System.Collections.Generic;
using GXPEngine.Entities;
using TiledMapParser;

namespace GXPEngine.StageManagement
{
    public class Stage : GameObject
    {
        private Map stageData;
        private int tileSize;
        
        public int stageWidth { get; private set; }
        public int stageHeight { get; private set; }

        public Stages stage { get; private set; }
        public Pivot stageContainer;

        /// <summary>
        /// Object that holds all information about the current stage including objects
        /// </summary>
        /// <param name="givenStage">A stage from the Stages.cs list</param>
        /// <exception cref="Exception">When the stage from Stages.cs doesn't have the same name as the file</exception>
        public Stage(Stages givenStage)
        {
            myGame.AddChild(this);
            
            stage = givenStage;
            string stagePath = "tiled/stages/" + stage.ToString() + ".tmx";
            stageData = MapParser.ReadMap(stagePath);
            
            //TileSize is the same as width and width is the same as height
            tileSize = stageData.TileWidth;

            if (stageData.Layers == null || stageData.Layers.Length <= 0)
            {
                throw new Exception("Tile file " + stagePath + " does not contain a layer!");
            }

            LoadStage();

        }
        void Update()
        {
            SortDisplayHierarchy();
        }

        /// <summary>
        /// Loads in the stage from tiled
        /// </summary>
        private void LoadStage()
        {
            stageWidth = stageData.Width * stageData.TileWidth;
            stageHeight = stageData.Height * stageData.TileHeight;
            
            Layer mainLayer = stageData.Layers[0];
            
            AddChild(new Sprite("background.png", addCollider:false));
            
            short [,] tileNumbers = mainLayer.GetTileArray();
            
            for (int col = 0; col < mainLayer.Width; col++)
            for (int row = 0; row < mainLayer.Height; row++)
            {
                int x = col * tileSize;
                int y = row * tileSize;
                
                switch (tileNumbers[col, row])
                {
                        
                    //Barriers
                    case 1:
                        Barrier barrier = new Barrier();
                        barrier.SetXY(x,y);
                        AddChild(barrier);
                        break;
                    
                    case 2:
                        myGame.player = new Player();
                        myGame.player.SetXY(x,y);
                        AddChild(myGame.player);
                        break;
                    
                    case 3:
                        AnimatedDecoration candy = new AnimatedDecoration("models/candy_cane.png", 24, 1, 50);
                        candy.SetXY(x - 128,y - 128);
                        candy.SetScaleXY(2.0f);
                        AddChild(candy);
                        break;
                    
                    case 4:
                        AnimatedDecoration lollipop = new AnimatedDecoration("models/lollipop.png", 24, 1, 50);
                        lollipop.SetXY(x-128,y-32);
                        lollipop.SetScaleXY(1.25f);
                        AddChild(lollipop);
                        break;
                    
                    case 5:
                        PizzaZombie pizzaZombie = new PizzaZombie();
                        pizzaZombie.SetXY(x,y);
                        AddChild(pizzaZombie);
                        break;
                    
                    case 6:
                        TomatoZombie tomatoZombie = new TomatoZombie();
                        tomatoZombie.SetXY(x,y);
                        AddChild(tomatoZombie);
                        break;
                }
            }

            //Adds a barrier to the left side of the stage
            Barrier leftBorder = new Barrier();
            leftBorder.SetXY(-64,0);
            leftBorder.width = 64;
            leftBorder.height = stageHeight;
            AddChild(leftBorder);

            //Adds a floor to the game
            Barrier ground = new Barrier();
            ground.SetXY(0,stageHeight);
            ground.width = stageWidth;
            ground.height = 64;
            AddChild(ground);
            
            //Adds a barrier to the right side of the map
            Barrier rightBorder = new Barrier();
            rightBorder.SetXY(stageWidth,0);
            rightBorder.width = 64;
            rightBorder.height = stageHeight;
            AddChild(rightBorder);
        }
        
        /// <summary>
        /// Sorts the display hierarchy to make sure objects are layered correctly.
        /// </summary>
        private void SortDisplayHierarchy()
        {
            List<GameObject> children = GetChildren();

            for (int i = children.Count-1; i >= 0; i--)
            {
                for (int j = i - 1; j >= 0; j--)
                {
                    if (!children[i].Equals(children[j]))
                    {
                        if (children[i].y < children[j].y)
                        {
                            SetChildIndex(children[j],i);
                        }
                    }
                }
            }
        }
        
        /// <returns>A list of all entities in this stage</returns>
        public List<Entity> GetEntities()
        {
            List<Entity> entities = new List<Entity>();
            foreach (GameObject gameObject in GetChildren())
            {
                if (gameObject is Entity entity)
                {
                    entities.Add(entity);
                }    
            }
            return entities;
        }

        /// <returns>A list of all enemies in this stage</returns>
        public List<Enemy> GetEnemies()
        {
            List<Enemy> enemies = new List<Enemy>();
            
            foreach (Entity entity in GetEntities())
            {
                if (entity is Enemy enemy)
                {
                    enemies.Add(enemy);
                }
            }
            return enemies;
        }
    }
}