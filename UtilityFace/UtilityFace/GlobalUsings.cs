﻿global using AcClient;
global using ACEditor.Props;
global using Decal.Adapter;
global using ImGuiNET;
global using System.Collections.Generic;
global using System.IO;
global using System.Linq;
global using System.Numerics;
global using System.Text.RegularExpressions;
global using System;
global using System.Drawing;
global using UtilityBelt.Common.Enums;
global using UtilityBelt.Scripting.Enums;
global using UtilityBelt.Scripting.Interop;
global using UtilityBelt.Service.Views;
global using UtilityBelt.Service;
global using UtilityFace.Components.Filters;
global using UtilityFace.Components.Modals;
global using UtilityFace.Components.Pickers;
global using UtilityFace.Helpers;
global using UtilityFace.Table;
global using UtilityFace;
global using UtilityBelt.Service.Lib.Settings;

global using Attribute = UtilityBelt.Scripting.Interop.Attribute;
global using Hud = UtilityBelt.Service.Views.Hud;
global using Position = UtilityBelt.Scripting.Interop.Position;
global using PropType = ACEditor.Props.PropType;
global using Skill = UtilityBelt.Scripting.Interop.Skill;
global using Vector3 = System.Numerics.Vector3;
global using Vector4 = System.Numerics.Vector4;
global using Quaternion = System.Numerics.Quaternion;
global using Vital = UtilityBelt.Scripting.Interop.Vital;
global using WorldObject = UtilityBelt.Scripting.Interop.WorldObject;
//Was confusing namespace/type for Settings
global using S = UtilityBelt.Service.Lib.Settings.Settings;