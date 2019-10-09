﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Game.Beatmaps.Formats;

namespace osu.Game.Skinning
{
    public class LegacySkinDecoder : LegacyDecoder<LegacySkinConfiguration>
    {
        public LegacySkinDecoder()
            : base(1)
        {
        }

        protected override void ParseLine(LegacySkinConfiguration skin, Section section, string line)
        {
            if (section != Section.Colours)
            {
                line = StripComments(line);

                var pair = SplitKeyVal(line);

                switch (section)
                {
                    case Section.General:
                        switch (pair.Key)
                        {
                            case @"Name":
                                skin.SkinInfo.Name = pair.Value;
                                return;

                            case @"Author":
                                skin.SkinInfo.Creator = pair.Value;
                                return;

                            case @"Version":
                                if (pair.Value == "latest" || pair.Value == "User")
                                    skin.LegacyVersion = LegacySkinConfiguration.LATEST_VERSION;
                                else if (decimal.TryParse(pair.Value, out var version))
                                    skin.LegacyVersion = version;

                                return;
                        }

                        break;
                }

                if (!string.IsNullOrEmpty(pair.Key))
                    skin.ConfigDictionary[pair.Key] = pair.Value;
            }

            base.ParseLine(skin, section, line);
        }
    }
}
