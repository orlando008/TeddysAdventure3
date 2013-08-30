using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace TeddysAdventureLibrary
{
    public static class SoundEffectHelper
    {
        public static void PlaySoundEffect(Game g, string soundEffectName)
        {
            SoundEffect se = g.Content.Load<SoundEffect>("Sounds\\" + soundEffectName);
            se.Play();
        }

        public static SoundEffectInstance GetSoundEffectInstance(Game g, string soundEffectName, bool loop)
        {
            SoundEffectInstance sei = GetSoundEffect(g, soundEffectName).CreateInstance();
            sei.IsLooped = loop;
            
            return sei;
        }

        public static SoundEffect GetSoundEffect(Game g, string soundEffectName)
        {
            SoundEffect se = g.Content.Load<SoundEffect>("Sounds\\" + soundEffectName);
            return se;
        }
    }
}
