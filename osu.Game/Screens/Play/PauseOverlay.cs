﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Linq;
using NUnit.Framework.Internal;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Audio;
using osu.Game.Audio;
using osu.Game.Graphics;
using osu.Game.Skinning;
using osuTK.Graphics;

namespace osu.Game.Screens.Play
{
    public class PauseOverlay : GameplayMenuOverlay
    {
        public Action OnResume;

        public override string Header => "paused";
        public override string Description => "you're not going to do what i think you're going to do, are ya?";

        private SkinnableSound pauseLoop;

        protected override Action BackAction => () => InternalButtons.Children.First().Click();

        [BackgroundDependencyLoader]
        private void load(OsuColour colours)
        {
            AddButton("Continue", colours.Green, () => OnResume?.Invoke());
            AddButton("Retry", colours.YellowDark, () => OnRetry?.Invoke());
            AddButton("Quit", new Color4(170, 27, 39, 255), () => OnQuit?.Invoke());

            AddInternal(pauseLoop = new SkinnableSound(new SampleInfo("pause-loop"))
            {
                Looping = true,
            });
            // PopIn is called before updating the skin, and when a sample is updated, its "playing" value is reset
            // the sample must be played again
            pauseLoop.OnSkinChanged += () => pauseLoop.Play();
        }

        protected override void PopIn()
        {
            base.PopIn();

            //SkinnableSound only plays a sound if its aggregate volume is > 0, so the volume must be turned up before playing it
            pauseLoop.VolumeTo(0.00001f);
            pauseLoop.VolumeTo(1.0f, 400, Easing.InQuint);
            pauseLoop.Play();
        }

        protected override void PopOut()
        {
            base.PopOut();

            var transformSeq = pauseLoop.VolumeTo(0.0f, 190, Easing.OutQuad );
            transformSeq.Finally(_ => pauseLoop.Stop());

        }
    }
}
