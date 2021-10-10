using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using Newtonsoft.Json;
using ImGuiNET;
using System.IO;


namespace HorizonEngine
{
    public static class Physics
    {
        [JsonObject(MemberSerialization.Fields)]
        private class PhysicsSettings
        {
            public Vector2 gravity;
            public PhysicsMaterialBlendMode frictionBlendMode;
            public PhysicsMaterialBlendMode restitutionBlendMode;
            public float defaultFriction;
            public float defaultRestitution;
        }

        private static Vector2 _gravity;
        private static int[] _ignoreMask;
        private static PhysicsMaterialBlendMode _frictionBlendMode;
        private static PhysicsMaterialBlendMode _restitutionBlendMode;
        private static float _defaultFriction;
        private static float _defaultRestitution;

        static Physics()
        {
            _ignoreMask = new int[32];
            _gravity = new Vector2(0, -9.8f);
            _defaultFriction = 0f;
            _defaultRestitution = 0f;
            LoadSettings();
        }

        public static Vector2 gravity
        {
            get
            {
                return _gravity;
            }
            set
            {
                _gravity = value;
            }
        }

        public static float defaultFriction
        {
            get
            {
                return _defaultFriction;
            }
            set
            {
                _defaultFriction = MathHelper.Clamp(value, 0f, 1f);
            }
        }

        public static float defaultRestitution
        {
            get
            {
                return _defaultRestitution;
            }
            set
            {
                _defaultRestitution = MathHelper.Clamp(value, 0f, 1f);
            }
        }

        public static PhysicsMaterialBlendMode frictionBlendMode
        {
            get
            {
                return _frictionBlendMode;
            }
            set
            {
                _frictionBlendMode = value;
            }
        }

        public static PhysicsMaterialBlendMode restitutionBlendMode
        {
            get
            {
                return _restitutionBlendMode;
            }
            set
            {
                _restitutionBlendMode = value;
            }
        }

        internal static int[] ignoreMask
        {
            get
            {
                return _ignoreMask;
            }
        }

        public static void IgnoreLayerCollision(Layer layer1, Layer layer2, bool ignore = true)
        {
            if(ignore)
            {
                _ignoreMask[(int)layer1] |= 1 << (int)layer2;
                _ignoreMask[(int)layer2] |= 1 << (int)layer1;
            }
            else
            {
                _ignoreMask[(int)layer1] &= ~(1 << (int)layer2);
                _ignoreMask[(int)layer2] &= ~(1 << (int)layer1);
            }
        }

        internal static void SaveSettings()
        {
            PhysicsSettings physicsSettings = new PhysicsSettings();
            physicsSettings.gravity = gravity;
            physicsSettings.frictionBlendMode = frictionBlendMode;
            physicsSettings.restitutionBlendMode = restitutionBlendMode;
            physicsSettings.defaultFriction = defaultFriction;
            physicsSettings.defaultRestitution = defaultRestitution;
            File.WriteAllText(Path.Combine(Application.projectPath, "PhysicsSettings.json"), JsonConvert.SerializeObject(physicsSettings));
        }

        internal static void LoadSettings()
        {
            PhysicsSettings physicsSettings = JsonConvert.DeserializeObject<PhysicsSettings>(File.ReadAllText(Path.Combine(Application.projectPath, "PhysicsSettings.json")));
            gravity = physicsSettings.gravity;
            frictionBlendMode = physicsSettings.frictionBlendMode;
            restitutionBlendMode = physicsSettings.restitutionBlendMode;
            defaultFriction = physicsSettings.defaultFriction;
            defaultRestitution = physicsSettings.defaultRestitution;
        }
    }
}
