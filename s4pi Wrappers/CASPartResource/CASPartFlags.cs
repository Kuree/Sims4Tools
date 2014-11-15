/***************************************************************************
 *  Copyright (C) 2009, 2010 by Peter L Jones                              *
 *  pljones@users.sf.net                                                   *
 *                                                                         *
 *  This file is part of the Sims 3 Package Interface (s3pi)               *
 *                                                                         *
 *  s3pi is free software: you can redistribute it and/or modify           *
 *  it under the terms of the GNU General Public License as published by   *
 *  the Free Software Foundation, either version 3 of the License, or      *
 *  (at your option) any later version.                                    *
 *                                                                         *
 *  s3pi is distributed in the hope that it will be useful,                *
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of         *
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the          *
 *  GNU General Public License for more details.                           *
 *                                                                         *
 *  You should have received a copy of the GNU General Public License      *
 *  along with s3pi.  If not, see <http://www.gnu.org/licenses/>.          *
 ***************************************************************************/
using System;
using System.IO;
using s4pi.Interfaces;
using System.Collections.Generic;

namespace CASPartResource
{
    [Flags]
    public enum AgeFlags : byte
    {
        Baby = 0x01,
        Toddler = 0x02,//"Pre-schooler"?
        Child = 0x04,
        Teen = 0x08,

        YoungAdult = 0x10,
        Adult = 0x20,
        Elder = 0x40,
    }

    public enum SpeciesType : byte
    {
        None = 0,
        Human = 0x1,
        Horse,
        Cat,

        Dog,
        LittleDog,
        Deer,
        Raccoon,

        LargeBird,
        SimWalkingDog,
        SimWalkingLittleDog,
        SimLeadingHorse,

        Paddleboat,
        WaterScooter,
        Speedboat,
        Rowboat,

        HouseboatSmall = 0x41,
        HouseboatMedium,
        HouseboatLarge,

        Shark,
        Sailboat,
        WindsurfBoard,
    }

    [Flags]
    public enum GenderFlags : byte
    {
        Male = 0x1,
        Female = 0x2,
    }

    [Flags]
    public enum HandednessFlags : byte
    {
        LeftHanded = 0x01,
        RightHanded = 0x02,
    }



    [Flags]
    public enum FacialRegionFlags : uint
    {
        Eyes = 0x001,
        Nose = 0x002,
        Mouth = 0x004,
        TranslateMouth = 0x008,
        Ears = 0x010,
        TranslateEyes = 0x020,
        Face = 0x040,
        Head = 0x080,
        Brow = 0x100,
        Jaw = 0x200,
        Body = 0x400,
        Eyelashes = 0x800,
    }

    [Flags]
    public enum DataTypeFlags : uint
    {
        Hair = 0x00000001,
        Scalp = 0x00000002,
        FaceOverlay = 0x00000004,
        Body = 0x00000008,
        Accessory = 0x00000010,
    }

    [Flags]
    public enum ClothingCategoryFlags : uint
    {
        Naked = 0x00000001,
        Everyday = 0x00000002,
        Formalwear = 0x00000004,
        Sleepwear = 0x00000008,

        Swimwear = 0x00000010,
        Athletic = 0x00000020,
        Singed = 0x00000040,
        MartialArts = 0x00000080,

        Career = 0x00000100,
        FireFighting = 0x00000200,
        Makeover = 0x00000400,
        SkinnyDippingTowel = 0x00000800,

        Racing = 0x00001000,
        Jumping = 0x00002000,
        Bridle = 0x00004000,
        //Markings = 0x00007000,
        Unknown0F = 0x00008000,

        Unknown10 = 0x00010000,
        Unknown11 = 0x00020000,
        Outerwear = 0x00040000,
        //CategoryMask = 0x0007FFFF,
        Plumbotwear = 0x00080000,

        //"Extended" flags
        ValidForMaternity = 0x00100000,
        ValidForRandom = 0x00200000,
        IsHat = 0x00400000,
        IsRevealing = 0x00800000,

        IsHiddenInCAS = 0x01000000,

        //  C BA9
        //  0 001x - head
        //  0 010x - chest
        //  0 011x - belly
        //  0 100x - back
        //  0 101x - tail
        //  0 110x - front left leg
        //  0 111x - front right leg
        //  1 000x - rear left leg
        //  1 001x - rear right leg
        //  1 010x - all
        //Unknown19 = 0x02000000,
        //Unknown1A = 0x04000000,
        //Unknown1B = 0x08000000,
        //Unknown1C = 0x10000000,
        RegionHead = 0x02000000,
        RegionChest = 0x04000000,
        RegionBelly = 0x06000000,
        RegionBack = 0x08000000,
        RegionTail = 0x0A000000,
        RegionFrontLeftLeg = 0x0C000000,
        RegionFrontRightLeg = 0x0E000000,
        RegionRearLeftLeg = 0x10000000,
        RegionRearRightLeg = 0x12000000,
        RegionAll = 0x14000000,
        //RegionMask = 0x1E000000,

        Unknown1D = 0x20000000,
        Unknown1E = 0x40000000,
        Unknown1F = 0x80000000,
    }

    public enum RegionType : byte
    {
        None = 0x00,
        Head = 0x01,
        Chest = 0x02,
        Belly = 0x03,
        Back = 0x04,
        Tail = 0x05,
        FrontLeftLeg = 0x06,
        FrontRightLeg = 0x07,
        RearLeftLeg = 0x08,
        RearRightLeg = 0x09,
        All = 0x0A,
    }
    public static class Extensions
    {
        public static RegionType GetRegionType(ClothingCategoryFlags value)
        {
            uint raw = (uint)value;
            raw >>= 25;
            return (RegionType)(raw & 0x0F);
        }
        public static ClothingCategoryFlags ToClothingCategoryFlags(RegionType value)
        {
            uint raw = (uint)value;
            return (ClothingCategoryFlags)(raw << 25);
        }
    }

    [Flags]
    public enum UnknownFlags : uint
    {
        Unknown00 = 0x00000001,
        Unknown01 = 0x00000002,
        Unknown02 = 0x00000004,
        Unknown03 = 0x00000008,

        Unknown04 = 0x00000010,
        Unknown05 = 0x00000020,
        Unknown06 = 0x00000040,
        Unknown07 = 0x00000080,

        Unknown08 = 0x00000100,
        Unknown09 = 0x00000200,
        Unknown0A = 0x00000400,
        Unknown0B = 0x00000800,

        Unknown0C = 0x00001000,
        Unknown0D = 0x00002000,
        Unknown0E = 0x00004000,
        Unknown0F = 0x00008000,

        Unknown10 = 0x00010000,
        Unknown11 = 0x00020000,
        Unknown12 = 0x00040000,
        Unknown13 = 0x00080000,

        Unknown14 = 0x00100000,
        Unknown15 = 0x00200000,
        Unknown16 = 0x00400000,
        Unknown17 = 0x00800000,

        Unknown18 = 0x01000000,
        Unknown19 = 0x02000000,
        Unknown1A = 0x04000000,
        Unknown1B = 0x08000000,

        Unknown1C = 0x10000000,
        Unknown1D = 0x20000000,
        Unknown1E = 0x40000000,
        Unknown1F = 0x80000000,
    }

    public enum ClothingType : uint
    {
        None = 0x00000000,
        Hair = 0x00000001,
        Scalp = 0x00000002,
        Face = 0x00000003,
        Body = 0x00000004,
        Top = 0x00000005,
        Bottom = 0x00000006,
        Shoes = 0x00000007,

        FirstAccessory = 0x00000008,
        Necklace = 0x00000009,
        NoseRing = 0x0000000A,
        Earrings = 0x0000000B,
        Glasses_F = 0x0000000C,
        Bracelets = 0x0000000D,
        Ring_Lt = 0x0000000E,
        Ring_Rt = 0x0000000F,

        Beard = 0x00000010,
        Lipstick = 0x00000011,
        Eyeshadow = 0x00000012,
        Eyeliner = 0x00000013,
        Blush = 0x00000014,
        Makeup = 0x00000015,
        Eyebrow = 0x00000016,
        EyeColor = 0x00000017,

        Glove = 0x00000018,
        Socks = 0x00000019,
        Mascara = 0x0000001A,
        Moles = 0x0000001B,
        Freckles = 0x0000001C,
        Weathering = 0x0000001D,
        EarringL = 0x0000001E,
        EarringR = 0x0000001F,

        ArmBand = 0x00000020,
        Tattoo = 0x00000021,
        TattooTemplate = 0x00000022,
        Dental = 0x00000023,
        LeftGarter = 0x00000024,
        RightGarter = 0x00000025,
        BirthMark = 0x00000026,
        BodyHairChestUpper = 0x00000027,

        BodyHairChestStomach = 0x00000028,
        BodyHairBackLower = 0x00000029,
        BodyHairBackUpper = 0x0000002A,
        BodyHairBackFull = 0x0000002B,
        BodyHairForearms = 0x0000002C,
        BodyHairLegsCalves = 0x0000002D,
        BodyHairLegsFeet = 0x0000002E,
        PetBody = 0x0000002F,

        PetTail = 0x00000030,
        PetEars = 0x00000031,
        PetMane = 0x00000032,
        PetCollarBridle = 0x00000033,
        PetSaddle = 0x00000034,
        PeltLayer = 0x00000035,
        BasePeltLayer = 0x00000036,
        PetHooves = 0x00000037,

        PetHorn = 0x00000038,
        PetBlanket = 0x00000039,
        PetBreastCollar = 0x0000003A,
        PetBeard = 0x0000003B,
        Unknown3C = 0x0000003C,
        Unknown3D = 0x0000003D,
        Unknown3E = 0x0000003E,
        Unknown3F = 0x0000003F,
    }

    [Flags]
    public enum CASGeomFlags : uint
    {
        Mergable = 0x00000001,
        IncludeMorphs = 0x00000002,
        IncludeTweaks = 0x00000004,
        IncludeTangents = 0x00000008,

        FourBoneSkinning = 0x00000010,
        TwoBoneSkinning = 0x00000020,
        TwoQuatSkinning = 0x00000040,
        OneQuatSkinning = 0x00000080,

        SpecLevel0 = 0x00000100,
        SpecLevel1 = 0x00000200,
        SpecLevel2 = 0x00000400,
        SpecLevel3 = 0x00000800,

        SpecLevel4 = 0x00001000,
        SpecLevel5 = 0x00002000,
        Sorted = 0x00004000,
        ShadowCaster = 0x00008000,
    }
}