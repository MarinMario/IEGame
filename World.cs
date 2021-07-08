﻿using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;

namespace Game
{
    class World : IScene
    {
        List<IBody> bodies = new List<IBody>();
        Player player = new Player();
        Camera2D camera = new Camera2D(Vector2.Zero, Vector2.Zero, 0, 1);
        List<MapObject> objects = LevelEditor.Load("test");
        List<Monster> monsters = new List<Monster>();

        Spider spider = new Spider();

        public World()
        {
            //bodies.Add(player);
            camera.target = player.Body.position;

            foreach (var obj in objects)
            {
                var type = Texture.MOTexture[obj.Name].type;
                if (type == MOType.CollisionTile)
                {
                    var pos = new Vector2(obj.X, obj.Y);
                    var texture = Texture.MOTexture[obj.Name].texture;
                    var size = new Vector2(texture.width, texture.height);
                    bodies.Add(new SimpleBody(new CollisionBody(pos, size)));
                }
                else if (type == MOType.Monster)
                {
                    monsters.Add(new Monster(new Vector2(obj.X, obj.Y)));
                    bodies.Add(monsters[^1]);
                }
            }
            
        }

        public void Update()
        {
            player.Update(bodies, camera.target - camera.offset);
            camera.offset = new Vector2(Raylib.GetScreenWidth() / 2 - spider.Body.size.X / 2, Raylib.GetScreenHeight()) ;
            camera.target = camera.target.MoveTowards(new Vector2(spider.Body.position.X, 0), Raylib.GetFrameTime() * 500, 1);

            if (Raylib.IsKeyDown(KeyboardKey.KEY_LEFT_CONTROL) && Raylib.IsKeyPressed(KeyboardKey.KEY_L))
                Program.scene = new LevelEditor();

            foreach (var m in monsters)
                m.Update(bodies);

            spider.Update(bodies);
        }

        public void Draw()
        {
            Raylib.BeginMode2D(camera);
            
            foreach (var o in objects)
                Raylib.DrawTextureEx(Texture.MOTexture[o.Name].texture, new Vector2(o.X, o.Y), 0, 1, Color.WHITE);

            foreach (var m in monsters)
                m.Draw();

            //player.Draw();

            //foreach (var body in bodies) body.Body.Draw(new Color(100, 0, 0, 100));

            spider.Draw();

            Raylib.EndMode2D();
        }


    }
}
