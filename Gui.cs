﻿using System;
using System.Collections.Generic;
using Raylib_cs;
using System.Numerics;

namespace Game
{
    class Button
    {
        public Vector2 position;
        public Vector2 size;
        public string text;
        public GuiStyle style;

        public Button(Vector2 position, Vector2 size, string text, GuiStyle style)
        {
            this.position = position;
            this.size = size;
            this.text = text;
            this.style = style;
        }

        public bool Hover()
        {
            var m = Raylib.GetMousePosition();
            return m.X > position.X && m.Y > position.Y && m.X < position.X + size.X && m.Y < position.Y + size.Y;
        }

        public bool Active()
        {
            return Raylib.IsMouseButtonDown(MouseButton.MOUSE_LEFT_BUTTON) && Hover();
        }

        public void Draw()
        {
            var s = Active() ? style.active : Hover() ? style.hover : style.normal;
            var textSize = Raylib.MeasureTextEx(s.font, text, s.fontSize, s.spacing);
            var textPos = position + size / 2 - textSize / 2;

            Raylib.DrawRectangleV(position, size, s.backgroundColor);
            Raylib.DrawRectangleLinesEx(new Rectangle(position.X, position.Y, size.X, size.Y), s.borderThickness, s.borderColor);
            Raylib.DrawTextEx(s.font, text, textPos, s.fontSize, s.spacing, s.fontColor);
        }
    }

    class TextBox
    {
        public Vector2 position;
        public Vector2 size;
        public string text;
        public GuiStyle style;
        bool active = false;
        float backSpaceTimerSincePressed = 0;
        float backSpaceTimer = 0;

        public TextBox(Vector2 position, Vector2 size, string text, GuiStyle style)
        {
            this.position = position;
            this.size = size;
            this.text = text;
            this.style = style;
        }

        public bool Hover()
        {
            var m = Raylib.GetMousePosition();
            return m.X > position.X && m.Y > position.Y && m.X < position.X + size.X && m.Y < position.Y + size.Y;
        }

        public bool Active()
        {
            return active;
        }

        public void Draw()
        {
            if (Raylib.IsMouseButtonPressed(MouseButton.MOUSE_LEFT_BUTTON))
                active = Hover();

            var s = Active() ? style.active : Hover() ? style.hover : style.normal;
            var textSize = Raylib.MeasureTextEx(s.font, text, s.fontSize, s.spacing);
            var textPos = position + size / 2 - textSize / 2;

            //logic for typing and deleting text in the textbox
            if (active)
            {
                if (text.Length > 0)
                {
                    if (Raylib.IsKeyPressed(KeyboardKey.KEY_BACKSPACE))
                    {
                        text = text[0..^1];
                        backSpaceTimerSincePressed = 0.01f;
                    }
                    if (backSpaceTimerSincePressed != 0)
                    {
                        backSpaceTimerSincePressed += Raylib.GetFrameTime();
                        if (backSpaceTimerSincePressed > 0.3)
                            backSpaceTimerSincePressed = 0;
                    }
                    if (Raylib.IsKeyDown(KeyboardKey.KEY_BACKSPACE) && backSpaceTimerSincePressed == 0)
                    {
                        backSpaceTimer += Raylib.GetFrameTime();
                        if (backSpaceTimer > 0.05)
                        {
                            text = text[0..^1];
                            backSpaceTimer = 0;
                        }
                    }
                    else
                    {
                        backSpaceTimer = 0;
                    }
                }

                var key = Raylib.GetCharPressed();
                if (key >= 32 && key <= 125 && textSize.X < size.X - 5 * s.borderThickness)
                    text += (char)key;
            }

            //drawing
            Raylib.DrawRectangleV(position, size, s.backgroundColor);
            Raylib.DrawRectangleLinesEx(new Rectangle(position.X, position.Y, size.X, size.Y), s.borderThickness, s.borderColor);
            Raylib.DrawTextEx(s.font, text, textPos, s.fontSize, s.spacing, s.fontColor);
        }
    }

    class GuiStyle
    {
        public Style normal = new Style();
        public Style hover = new Style();
        public Style active = new Style();

        public static GuiStyle Default()
        {
            var s = new GuiStyle();

            var bgColor = new Color(255, 226, 104, 255);
            var borderColor = new Color(54, 69, 71, 255);
            var borderColorHover = new Color(35, 45, 46, 255);
            var bgColorActive = new Color(255, 176, 55, 255);

            s.normal.fontColor = borderColor;
            s.normal.backgroundColor = bgColor;
            s.normal.borderColor = borderColor;

            s.hover.fontColor = borderColorHover;
            s.hover.backgroundColor = bgColor;
            s.hover.borderColor = borderColorHover;

            s.active.fontColor = borderColorHover;
            s.active.backgroundColor = bgColorActive;
            s.active.borderColor = borderColorHover;

            return s;
        }

        public class Style
        {
            public Font font = Raylib.GetFontDefault();
            public Color fontColor = Color.BLACK;
            public int fontSize = 20;
            public int spacing = 1;
            public Color backgroundColor = Color.LIGHTGRAY;
            public Color borderColor = Color.BLACK;
            public int borderThickness = 5;
        }
    }
}
