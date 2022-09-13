using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using Color = Microsoft.Xna.Framework.Color;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace TICMod.UI
{
    // ty jopojelly
    internal class UIBetterTextBox : UIPanel//UITextPanel<string>
    {
        internal bool focused = false;

        //private int _cursor;
        //private int _frameCount;
        private int _maxLength = Int32.MaxValue;

        private string hintText;
        internal Stack<String> history = new Stack<string>();
        internal string currentString = "";
        private int textBlinkerCount;
        private int textBlinkerState;

        public event Action OnFocus;

        public event Action OnUnfocus;

        public event Action OnTextChanged;

        public event Action OnTabPressed;

        public event Action OnEnterPressed;

        public Color textColor = Color.Black;

        //public event Action OnUpPressed;
        internal bool unfocusOnEnter = true;

        internal bool unfocusOnTab = true;

        public string hoverText;

        private int cursorPos = 0;

        //public NewUITextBox(string text, float textScale = 1, bool large = false) : base("", textScale, large)
        public UIBetterTextBox(string hintText, string text = "")
        {
            this.hintText = hintText;
            currentString = text;
            SetPadding(0);
            BackgroundColor = Color.White;
            BorderColor = Color.Black;
            //			keyBoardInput.newKeyEvent += KeyboardInput_newKeyEvent;
        }

        public override void Click(UIMouseEvent evt)
        {
            // Get the absolute position of this element
            float absPos = Left.Pixels;
            UIElement examining = Parent;
            while (examining != null)
            {
                absPos += examining.Left.Pixels;
                examining = examining.Parent;
            }

            // Get the distance from the start of this element
            float leftDistance = evt.MousePosition.X - absPos - 20;
            
            // Find the character closest to that position
            List<float> charPos = new List<float>();
            charPos.Add(0);
            string builtString = "";
            foreach (char c in currentString)
            {
                builtString += c;
                charPos.Add(FontAssets.MouseText.Value.MeasureString(builtString).X);
            }
            float closest = charPos.OrderBy(x => Math.Abs(leftDistance - x)).First();
            int index = charPos.IndexOf(closest);

            // Set the cursor to that character, and compensate for cursor char
            if (cursorPos < index && focused)
            {
                index--;
            }
            cursorPos = index;

            Focus();
            base.Click(evt);
        }

        public override void RightClick(UIMouseEvent evt)
        {
           Click(evt);
        }

        public void SetUnfocusKeys(bool unfocusOnEnter, bool unfocusOnTab)
        {
            this.unfocusOnEnter = unfocusOnEnter;
            this.unfocusOnTab = unfocusOnTab;
        }

        public void Unfocus()
        {
            if (focused)
            {
                focused = false;
                Main.blockInput = false;

                OnUnfocus?.Invoke();
            }
        }

        public void Focus()
        {
            if (!focused)
            {
                Main.clrInput();
                focused = true;
                Main.blockInput = true;
                //cursorPos = currentString.Length;

                OnFocus?.Invoke();
            }
        }

        public override void Update(GameTime gameTime)
        {
            Vector2 MousePosition = new Vector2((float)Main.mouseX, (float)Main.mouseY);
            if (!ContainsPoint(MousePosition) && (Main.mouseLeft || Main.mouseRight)) // This solution is fine, but we need a way to cleanly "unload" a UIElement
            {
                Unfocus();
            }
            base.Update(gameTime);
        }

        public void SetText(string text)
        {
            if (text.ToString().Length > this._maxLength)
            {
                text = text.ToString().Substring(0, this._maxLength);
            }
            if (currentString != text)
            {
                currentString = text;
                OnTextChanged?.Invoke();
            }
        }

        public void SetTextMaxLength(int maxLength)
        {
            this._maxLength = maxLength;
        }

        int backspaceThrottle = 0;
        int deleteThrottle = 0;
        int leftThrottle = 0;
        int rightTrottle = 0;
        int throttle = 6;
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            Rectangle hitbox = GetInnerDimensions().ToRectangle();

            // Draw panel
            base.DrawSelf(spriteBatch);
            //	Main.spriteBatch.Draw(Main.magicPixel, hitbox, Color.Yellow);

            if (focused)
            {
                Terraria.GameInput.PlayerInput.WritingText = true;
                Main.instance.HandleIME();

                string newString = Main.GetInputText("");
                if (newString != null && newString != "")
                {
                    history.Push(currentString);
                    currentString = currentString.Insert(cursorPos, newString);
                    cursorPos += newString.Length;
                    OnTextChanged?.Invoke();
                }

                if (Utils.JustPressed(Keys.Tab))
                {
                    if (unfocusOnTab) Unfocus();
                    OnTabPressed?.Invoke();
                }
                else if (Utils.JustPressed(Keys.Enter))
                {
                    Main.drawingPlayerChat = false;
                    if (unfocusOnEnter) Unfocus();
                    OnEnterPressed?.Invoke();
                }
                else if (Utils.IsPressed(Keys.Left))
                {
                    if (leftThrottle == 0)
                    {
                        cursorPos -= 1;
                        if (cursorPos < 0)
                        {
                            cursorPos = 0;
                        }
                    }

                    leftThrottle = ++leftThrottle % throttle;
                }
                else if (Utils.IsPressed(Keys.Right))
                {
                    if (rightTrottle == 0)
                    {
                        cursorPos += 1;
                        if (cursorPos > currentString.Length)
                        {
                            cursorPos = currentString.Length;
                        }
                    }

                    rightTrottle = ++rightTrottle % throttle;
                }
                else if (Utils.JustPressed(Keys.End))
                {
                    cursorPos = currentString.Length;
                }
                else if (Utils.JustPressed(Keys.Home))
                {
                    cursorPos = 0;
                }
                else if (Utils.IsPressed(Keys.Back) && cursorPos != 0)
                {
                    if (backspaceThrottle == 0)
                    {
                        history.Push(currentString);
                        currentString = currentString.Remove(cursorPos - 1, 1);
                        OnTextChanged?.Invoke();
                        cursorPos -= 1;
                    }

                    backspaceThrottle++;
                    backspaceThrottle = backspaceThrottle % throttle;
                }
                else if (Utils.IsPressed(Keys.Delete) && cursorPos != currentString.Length)
                {
                    if (deleteThrottle == 0)
                    {
                        history.Push(currentString);
                        currentString = currentString.Remove(cursorPos, 1);
                        OnTextChanged?.Invoke();
                    }

                    deleteThrottle++;
                    deleteThrottle = deleteThrottle % throttle;
                }
                else if (Utils.IsPressed(Keys.LeftControl) && Utils.JustPressed(Keys.C))
                {
                    ReLogic.OS.Platform.Current.Clipboard = currentString;

                }
                else if (Utils.IsPressed(Keys.LeftControl) && Utils.JustPressed(Keys.X))
                {
                    history.Push(currentString);
                    ReLogic.OS.Platform.Current.Clipboard = currentString;
                    OnTextChanged?.Invoke();
                    currentString = "";
                    cursorPos = 0;
                }
                else if (Utils.IsPressed(Keys.LeftControl) && Utils.JustPressed(Keys.V))
                {
                    history.Push(currentString);
                    currentString = currentString.Insert(cursorPos, ReLogic.OS.Platform.Current.Clipboard);
                    cursorPos += ReLogic.OS.Platform.Current.Clipboard.Length;
                    OnTextChanged?.Invoke();
                }
                else if (Utils.IsPressed(Keys.LeftControl) && Utils.JustPressed(Keys.Z) && history.Count > 0)
                {
                    currentString = history.Pop();
                    cursorPos = currentString.Length;
                }
                else
                {
                    backspaceThrottle = 0;
                    deleteThrottle = 0;
                }


                if (++textBlinkerCount >= 20)
                {
                    textBlinkerState = (textBlinkerState + 1) % 2;
                    textBlinkerCount = 0;
                }

                Main.instance.DrawWindowsIMEPanel(new Vector2(98f, (float)(Main.screenHeight - 36)), 0f);
            }

            string displayString = currentString;
            string cursorChar = " ";
            if (textBlinkerState == 1)
            {
                cursorChar = "|";
            }

            if (focused)
            {
                displayString = displayString.Insert(cursorPos, cursorChar);
            }

            CalculatedStyle space = base.GetDimensions();
            Color color = textColor;
            if (currentString.Length == 0)
            {
            }

            Vector2 drawPos = space.Position() + new Vector2(4, 2);
            if (currentString.Length == 0 && !focused)
            {
                color *= 0.5f;
                //Utils.DrawBorderString(spriteBatch, hintText, new Vector2(space.X, space.Y), Color.Gray, 1f);
                spriteBatch.DrawString(FontAssets.MouseText.Value, hintText, drawPos, color);
            }
            else
            {
                //Utils.DrawBorderString(spriteBatch, displayString, drawPos, Color.White, 1f);
                spriteBatch.DrawString(FontAssets.MouseText.Value, displayString, drawPos, color);
            }

            if (IsMouseHovering)
            {
                Main.hoverItemName = hoverText;
            }
        }
    }
}