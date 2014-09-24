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
using System.Collections.Generic;
using System.IO;
using s4pi.Interfaces;
using System.Text;

namespace CatalogResource
{
    public class ObjectCatalogResource : CatalogResourceTGIBlockList
    {
        #region Attributes
        //--Version
        //--TGIBlockList offset and size
        MaterialList materialList = null;
        string instanceName = "";//Version>=0x16
        //--CommonBlock
        uint objkIndex;
        ObjectType objectTypeFlags;
        ObjectTypeExt objectTypeFlags2;//Version>=0x1a
        WallPlacement wallPlacementFlags;
        Movement movementFlags;
        uint wallCutoutTilesPerLevel;
        uint levels;
        WallCutoutList wallCutoutList = null;
        bool isScriptEnabled;
        uint diagonalIndex;
        uint ambienceTypeHash;
        RoomCategory roomCategoryFlags;
        FunctionCategory functionCategoryFlags;
        FunctionSubCategory functionSubCategoryFlags;
        FunctionSubCategory2 functionSubCategoryFlags2;//Version>=1c
        RoomSubCategory roomSubCategoryFlags;
        BuildCategory buildCategoryFlags;
        uint surfaceCutoutDDSIndex;
        uint floorCutoutDDSIndex;//Version>=0x17
        uint floorCutoutLevelOffset;//Version>=0x17
        float floorCutoutBoundsLength;//Version>=0x17
        float floorCutoutBoundsWidth;//Version>=0x20
        float floorCutoutOffsetX;//Version>=0x21
        float floorCutoutOffsetZ;//Version>=0x21
        UIntList buildableShellDisplayStateHashes;//Version>=0x18
        uint levelBelowOBJDIndex;//Version>=0x19
        uint proxyOBJDIndex;//Version>=1b
        uint blueprintXMLIndex;//Version>=1d
        uint blueprintIconIndex;//Version>=1e
        float blueprintIconOffsetMinX;//Version>=1f
        float blueprintIconOffsetMinZ;//Version>=1f
        float blueprintIconOffsetMaxX;//Version>=1f
        float blueprintIconOffsetMaxZ;//Version>=1f
        SlotPlacement slotPlacementFlags;
        string surfaceType = "";
        string sourceMaterial = "";
        Moodlet moodletGiven;
        int moodletScore;
        uint unknown21;
        TopicRating[] topicRatings = new TopicRating[5];
        uint fallbackIndex;
        uint modularArchEndEastVPXYIndex;//Version>=0x22
        uint modularArchEndWestVPXYIndex;//Version>=0x22
        uint modularArchConnectVPXYIndex;//Version>=0x22
        uint modularArchSingleVPXYIndex;//Version>=0x22
        #endregion

        #region Constructors
        public ObjectCatalogResource(int APIversion, Stream s) : base(APIversion, s) { }
        public ObjectCatalogResource(int APIversion, Stream unused, ObjectCatalogResource basis)
            : base(APIversion, basis.version, basis.common, basis.list)
        {
            this.materialList = new MaterialList(OnResourceChanged, basis.materialList);
            this.instanceName = (this.version >= 0x00000016) ? basis.instanceName : null;
            //this.common = new Common(requestedApiVersion, OnResourceChanged, basis.common);
            this.objkIndex = basis.objkIndex;
            this.objectTypeFlags = basis.objectTypeFlags;
            this.objectTypeFlags2 = (this.version >= 0x0000001a) ? basis.objectTypeFlags2 : 0;
            this.wallPlacementFlags = basis.wallPlacementFlags;
            this.movementFlags = basis.movementFlags;
            this.wallCutoutTilesPerLevel = basis.wallCutoutTilesPerLevel;
            this.levels = basis.levels;
            this.wallCutoutList = new WallCutoutList(OnResourceChanged, basis.wallCutoutList, list);
            this.isScriptEnabled = basis.isScriptEnabled;
            this.diagonalIndex = basis.diagonalIndex;
            this.ambienceTypeHash = basis.ambienceTypeHash;
            this.roomCategoryFlags = basis.roomCategoryFlags;
            this.functionCategoryFlags = basis.functionCategoryFlags;
            this.functionSubCategoryFlags = basis.functionSubCategoryFlags;
            this.functionSubCategoryFlags2 = (this.version >= 0x0000001a) ? basis.functionSubCategoryFlags2 : 0;
            this.roomSubCategoryFlags = basis.roomSubCategoryFlags;
            this.buildCategoryFlags = basis.buildCategoryFlags;
            this.surfaceCutoutDDSIndex = basis.surfaceCutoutDDSIndex;
            this.floorCutoutDDSIndex = (this.version >= 0x00000017) ? basis.floorCutoutDDSIndex : 0;
            this.floorCutoutLevelOffset = (this.version >= 0x00000017) ? basis.floorCutoutLevelOffset : 0;
            this.floorCutoutBoundsLength = (this.version >= 0x00000017) ? basis.floorCutoutBoundsLength : 0;
            this.buildableShellDisplayStateHashes = (this.version >= 0x00000018) ? new UIntList(OnResourceChanged, basis.buildableShellDisplayStateHashes) : null;
            this.levelBelowOBJDIndex = (this.version >= 0x00000019) ? basis.levelBelowOBJDIndex : 0;
            this.proxyOBJDIndex = (this.version >= 0x0000001b) ? basis.proxyOBJDIndex : 0;
            this.slotPlacementFlags = basis.slotPlacementFlags;
            this.surfaceType = basis.surfaceType;
            this.sourceMaterial = basis.sourceMaterial;
            this.moodletGiven = basis.moodletGiven;
            this.moodletScore = basis.moodletScore;
            this.unknown21 = basis.unknown21;
            this.topicRatings = (TopicRating[])basis.topicRatings.Clone();
            this.fallbackIndex = basis.fallbackIndex;
        }

        // Current version 0x22
        public ObjectCatalogResource(int APIversion, uint version, IEnumerable<Material> materialList,
            string instanceName,//Version>=0x16
            Common common, uint objkIndex, ObjectType objectTypeFlags,
            ObjectTypeExt objectTypeFlags2,//Version>=0x1a
            WallPlacement wallPlacementFlags, Movement movementFlags, uint cutoutTilesPerLevel, uint levels, IEnumerable<WallCutout> mtDoorList,
            bool isScriptEnabled, uint diagonalIndex, uint ambienceTypeHash, RoomCategory roomCategoryFlags,
            FunctionCategory functionCategoryFlags, FunctionSubCategory functionSubCategoryFlags,
            FunctionSubCategory2 functionSubCategoryFlags2,//Version>=0x1c
            RoomSubCategory roomSubCategoryFlags, BuildCategory buildCategoryFlags, uint surfaceCutoutDDSIndex,
            uint floorCutoutDDSIndex, uint floorCutoutLevelOffset, float floorCutoutBoundsLength,//Version>=0x17
            float floorCutoutBoundsWidth,//Version>=0x20
            float floorCutoutOffsetX, float floorCutoutOffsetZ,//Version>=0x21
            IEnumerable<uint> buildableShellDisplayStateHashes,//Version>=0x18
            uint levelBelowOBJDIndex,//Version>=0x19
            uint proxyOBJDIndex,//Version>=0x1b
            uint blueprintXMLIndex,//Version>=0x1d
            uint blueprintIconIndex,//Version>=0x1e
            float blueprintIconOffsetMinX, float blueprintIconOffsetMinZ, float blueprintIconOffsetMaxX, float blueprintIconOffsetMaxZ,//Version>=0x1f
            SlotPlacement slotPlacementFlags, string surfaceType, string sourceMaterial, Moodlet moodletGiven, int moodletScore,
            uint unknown21, TopicRating[] topicRatings, uint fallbackIndex,
            uint modularArchEndEastVPXYIndex, uint modularArchEndWestVPXYIndex, uint modularArchConnectVPXYIndex, uint modularArchSingleVPXYIndex,//Version>=0x22
            IEnumerable<TGIBlock> ltgib)
            : base(APIversion, version, common, ltgib)
        {
            this.materialList = materialList == null ? null : new MaterialList(OnResourceChanged, materialList);
            this.instanceName = instanceName;
            //this.common = common == null ? null : new Common(requestedApiVersion, OnResourceChanged, common);
            this.objkIndex = objkIndex;
            this.objectTypeFlags = objectTypeFlags;
            this.objectTypeFlags2 = objectTypeFlags2;
            this.wallPlacementFlags = wallPlacementFlags;
            this.movementFlags = movementFlags;
            this.wallCutoutTilesPerLevel = cutoutTilesPerLevel;
            this.levels = levels;
            this.wallCutoutList = mtDoorList == null ? null : new WallCutoutList(OnResourceChanged, mtDoorList, list);
            this.isScriptEnabled = isScriptEnabled;
            this.diagonalIndex = diagonalIndex;
            this.ambienceTypeHash = ambienceTypeHash;
            this.roomCategoryFlags = roomCategoryFlags;
            this.functionCategoryFlags = functionCategoryFlags;
            this.functionSubCategoryFlags = functionSubCategoryFlags;
            this.functionSubCategoryFlags2 = functionSubCategoryFlags2;
            this.roomSubCategoryFlags = roomSubCategoryFlags;
            this.buildCategoryFlags = buildCategoryFlags;
            this.surfaceCutoutDDSIndex = surfaceCutoutDDSIndex;
            this.floorCutoutDDSIndex = floorCutoutDDSIndex;
            this.floorCutoutLevelOffset = floorCutoutLevelOffset;
            this.floorCutoutBoundsLength = floorCutoutBoundsLength;
            this.floorCutoutBoundsWidth = floorCutoutBoundsWidth;
            this.floorCutoutOffsetX = floorCutoutOffsetX;
            this.floorCutoutOffsetZ = floorCutoutOffsetZ;
            this.buildableShellDisplayStateHashes = buildableShellDisplayStateHashes == null ? null : new UIntList(OnResourceChanged, buildableShellDisplayStateHashes);
            this.levelBelowOBJDIndex = levelBelowOBJDIndex;
            this.proxyOBJDIndex = proxyOBJDIndex;
            this.slotPlacementFlags = slotPlacementFlags;
            this.surfaceType = surfaceType;
            this.sourceMaterial = sourceMaterial;
            this.moodletGiven = moodletGiven;
            this.moodletScore = moodletScore;
            this.unknown21 = unknown21;
            if (checking) if (topicRatings.Length != 5)
                    throw new ArgumentLengthException("TopicRatings", this.topicRatings.Length);
            this.topicRatings = (TopicRating[])topicRatings.Clone();
            this.fallbackIndex = fallbackIndex;
            this.modularArchEndEastVPXYIndex = modularArchEndEastVPXYIndex;
            this.modularArchEndWestVPXYIndex = modularArchEndWestVPXYIndex;
            this.modularArchConnectVPXYIndex = modularArchConnectVPXYIndex;
            this.modularArchSingleVPXYIndex = modularArchSingleVPXYIndex;
        }

        // Version 0x21
        public ObjectCatalogResource(int APIversion, uint version, IEnumerable<Material> materialList,
            string instanceName,//Version>=0x16
            Common common, uint objkIndex, ObjectType objectTypeFlags,
            ObjectTypeExt objectTypeFlags2,//Version>=0x1a
            WallPlacement wallPlacementFlags, Movement movementFlags, uint cutoutTilesPerLevel, uint levels, IEnumerable<WallCutout> mtDoorList,
            bool isScriptEnabled, uint diagonalIndex, uint ambienceTypeHash, RoomCategory roomCategoryFlags,
            FunctionCategory functionCategoryFlags, FunctionSubCategory functionSubCategoryFlags,
            FunctionSubCategory2 functionSubCategoryFlags2,//Version>=0x1c
            RoomSubCategory roomSubCategoryFlags, BuildCategory buildCategoryFlags, uint surfaceCutoutDDSIndex,
            uint floorCutoutDDSIndex, uint floorCutoutLevelOffset, float floorCutoutBoundsLength,//Version>=0x17
            float floorCutoutBoundsWidth,//Version>=0x20
            IEnumerable<uint> buildableShellDisplayStateHashes,//Version>=0x18
            float floorCutoutOffsetX, float floorCutoutOffsetZ,//Version>=0x21
            uint levelBelowOBJDIndex,//Version>=0x19
            uint proxyOBJDIndex,//Version>=0x1b
            uint blueprintXMLIndex,//Version>=0x1d
            uint blueprintIconIndex,//Version>=0x1e
            float blueprintIconOffsetMinX, float blueprintIconOffsetMinZ, float blueprintIconOffsetMaxX, float blueprintIconOffsetMaxZ,//Version>=0x1f
            SlotPlacement slotPlacementFlags, string surfaceType, string sourceMaterial, Moodlet moodletGiven, int moodletScore,
            uint unknown21, TopicRating[] topicRatings, uint fallbackIndex,
            //uint modularArchEndEastVPXYIndex, uint modularArchEndWestVPXYIndex, uint modularArchConnectVPXYIndex, uint modularArchSingleVPXYIndex,//Version>=0x22
            IEnumerable<TGIBlock> ltgib)
            : this(APIversion, version, materialList,
            instanceName,
            common, objkIndex, objectTypeFlags,
            objectTypeFlags2,
            wallPlacementFlags, movementFlags, cutoutTilesPerLevel, levels, mtDoorList,
            isScriptEnabled, diagonalIndex, ambienceTypeHash, roomCategoryFlags,
            functionCategoryFlags, functionSubCategoryFlags,
            functionSubCategoryFlags2,
            roomSubCategoryFlags, buildCategoryFlags, surfaceCutoutDDSIndex,
            floorCutoutDDSIndex, floorCutoutLevelOffset, floorCutoutBoundsLength,
            floorCutoutBoundsWidth,
            floorCutoutOffsetX, floorCutoutOffsetZ,
            buildableShellDisplayStateHashes,
            levelBelowOBJDIndex,
            proxyOBJDIndex,
            blueprintXMLIndex,
            blueprintIconIndex,
            blueprintIconOffsetMinX, blueprintIconOffsetMinZ, blueprintIconOffsetMaxX, blueprintIconOffsetMaxZ,
            slotPlacementFlags, surfaceType, sourceMaterial, moodletGiven, moodletScore,
            unknown21, topicRatings, fallbackIndex,
            0, 0, 0, 0,//Version>=0x22
            ltgib)
        {
            if (checking) if (version > 0x00000021)
                    throw new InvalidOperationException(String.Format("Constructor requires Modular Arch VPXY indexes for version {0}", version));
        }


        // Version 0x20
        public ObjectCatalogResource(int APIversion, uint version, IEnumerable<Material> materialList,
            string instanceName,//Version>=0x16
            Common common, uint objkIndex, ObjectType objectTypeFlags,
            ObjectTypeExt objectTypeFlags2,//Version>=0x1a
            WallPlacement wallPlacementFlags, Movement movementFlags, uint cutoutTilesPerLevel, uint levels, IEnumerable<WallCutout> mtDoorList,
            bool isScriptEnabled, uint diagonalIndex, uint ambienceTypeHash, RoomCategory roomCategoryFlags,
            FunctionCategory functionCategoryFlags, FunctionSubCategory functionSubCategoryFlags,
            FunctionSubCategory2 functionSubCategoryFlags2,//Version>=0x1c
            RoomSubCategory roomSubCategoryFlags, BuildCategory buildCategoryFlags, uint surfaceCutoutDDSIndex,
            uint floorCutoutDDSIndex, uint floorCutoutLevelOffset, float floorCutoutBoundsLength,//Version>=0x17
            float floorCutoutBoundsWidth,//Version>=0x20
            //float floorCutoutOffsetX, float floorCutoutOffsetZ,//Version>=0x21
            IEnumerable<uint> buildableShellDisplayStateHashes,//Version>=0x18
            uint levelBelowOBJDIndex,//Version>=0x19
            uint proxyOBJDIndex,//Version>=0x1b
            uint blueprintXMLIndex,//Version>=0x1d
            uint blueprintIconIndex,//Version>=0x1e
            float blueprintIconOffsetMinX, float blueprintIconOffsetMinZ, float blueprintIconOffsetMaxX, float blueprintIconOffsetMaxZ,//Version>=0x1f
            SlotPlacement slotPlacementFlags, string surfaceType, string sourceMaterial, Moodlet moodletGiven, int moodletScore,
            uint unknown21, TopicRating[] topicRatings, uint fallbackIndex,
            //uint modularArchEndEastVPXYIndex, uint modularArchEndWestVPXYIndex, uint modularArchConnectVPXYIndex, uint modularArchSingleVPXYIndex,//Version>=0x22
            IEnumerable<TGIBlock> ltgib)
            : this(APIversion, version, materialList,
            instanceName,
            common, objkIndex, objectTypeFlags,
            objectTypeFlags2,
            wallPlacementFlags, movementFlags, cutoutTilesPerLevel, levels, mtDoorList,
            isScriptEnabled, diagonalIndex, ambienceTypeHash, roomCategoryFlags,
            functionCategoryFlags, functionSubCategoryFlags,
            functionSubCategoryFlags2,
            roomSubCategoryFlags, buildCategoryFlags, surfaceCutoutDDSIndex,
            floorCutoutDDSIndex, floorCutoutLevelOffset, floorCutoutBoundsLength,
            floorCutoutBoundsWidth,
            0f, 0f,//Version>=0x21
            buildableShellDisplayStateHashes,
            levelBelowOBJDIndex,
            proxyOBJDIndex,
            blueprintXMLIndex,
            blueprintIconIndex,
            blueprintIconOffsetMinX, blueprintIconOffsetMinZ, blueprintIconOffsetMaxX, blueprintIconOffsetMaxZ,
            slotPlacementFlags, surfaceType, sourceMaterial, moodletGiven, moodletScore,
            unknown21, topicRatings, fallbackIndex,
            0, 0, 0, 0,//Version>=0x22
            ltgib)
        {
            if (checking) if (version > 0x00000020)
                    throw new InvalidOperationException(String.Format("Constructor requires Unknown22/23 and VPXY indexes for version {0}", version));
        }

        // Version 0x1f
        public ObjectCatalogResource(int APIversion, uint version, IEnumerable<Material> materialList,
            string instanceName,//Version>=0x16
            Common common, uint objkIndex, ObjectType objectTypeFlags,
            ObjectTypeExt objectTypeFlags2,//Version>=0x1a
            WallPlacement wallPlacementFlags, Movement movementFlags, uint cutoutTilesPerLevel, uint levels, IEnumerable<WallCutout> mtDoorList,
            bool isScriptEnabled, uint diagonalIndex, uint ambienceTypeHash, RoomCategory roomCategoryFlags,
            FunctionCategory functionCategoryFlags, FunctionSubCategory functionSubCategoryFlags,
            FunctionSubCategory2 functionSubCategoryFlags2,//Version>=0x1c
            RoomSubCategory roomSubCategoryFlags, BuildCategory buildCategoryFlags, uint surfaceCutoutDDSIndex,
            uint floorCutoutDDSIndex, uint floorCutoutLevelOffset, float floorCutoutBoundsLength,//Version>=0x17
            //float floorCutoutBoundsWidth,//Version>=0x20
            //float floorCutoutOffsetX, float floorCutoutOffsetZ,//Version>=0x21
            IEnumerable<uint> buildableShellDisplayStateHashes,//Version>=0x18
            uint levelBelowOBJDIndex,//Version>=0x19
            uint proxyOBJDIndex,//Version>=0x1b
            uint blueprintXMLIndex,//Version>=0x1d
            uint blueprintIconIndex,//Version>=0x1e
            float blueprintIconOffsetMinX, float blueprintIconOffsetMinZ, float blueprintIconOffsetMaxX, float blueprintIconOffsetMaxZ,//Version>=0x1f
            SlotPlacement slotPlacementFlags, string surfaceType, string sourceMaterial, Moodlet moodletGiven, int moodletScore,
            uint unknown21, TopicRating[] topicRatings, uint fallbackIndex,
            //uint modularArchEndEastVPXYIndex, uint modularArchEndWestVPXYIndex, uint modularArchConnectVPXYIndex, uint modularArchSingleVPXYIndex,//Version>=0x22
            IEnumerable<TGIBlock> ltgib)
            : this(APIversion, version, materialList,
            instanceName,
            common, objkIndex, objectTypeFlags,
            objectTypeFlags2,
            wallPlacementFlags, movementFlags, cutoutTilesPerLevel, levels, mtDoorList,
            isScriptEnabled, diagonalIndex, ambienceTypeHash, roomCategoryFlags,
            functionCategoryFlags, functionSubCategoryFlags,
            functionSubCategoryFlags2,
            roomSubCategoryFlags, buildCategoryFlags, surfaceCutoutDDSIndex,
            floorCutoutDDSIndex, floorCutoutLevelOffset, floorCutoutBoundsLength,
            0f,//Version>=0x20
            0f, 0f,//Version>=0x21
            buildableShellDisplayStateHashes,
            levelBelowOBJDIndex,
            proxyOBJDIndex,
            blueprintXMLIndex,
            blueprintIconIndex,
            blueprintIconOffsetMinX, blueprintIconOffsetMinZ, blueprintIconOffsetMaxX, blueprintIconOffsetMaxZ,
            slotPlacementFlags, surfaceType, sourceMaterial, moodletGiven, moodletScore,
            unknown21, topicRatings, fallbackIndex,
            0, 0, 0, 0,//Version>=0x22
            ltgib)
        {
            if (checking) if (version > 0x0000001f)
                    throw new InvalidOperationException(String.Format("Constructor requires FloorCutoutBoundsWidth for version {0}", version));
        }

        // Version 0x1e
        public ObjectCatalogResource(int APIversion, uint version, IEnumerable<Material> materialList,
            string instanceName,//Version>=0x16
            Common common, uint objkIndex, ObjectType objectTypeFlags,
            ObjectTypeExt objectTypeFlags2,//Version>=0x1a
            WallPlacement wallPlacementFlags, Movement movementFlags, uint cutoutTilesPerLevel, uint levels, IEnumerable<WallCutout> mtDoorList,
            bool isScriptEnabled, uint diagonalIndex, uint ambienceTypeHash, RoomCategory roomCategoryFlags,
            FunctionCategory functionCategoryFlags, FunctionSubCategory functionSubCategoryFlags,
            FunctionSubCategory2 functionSubCategoryFlags2,//Version>=0x1c
            RoomSubCategory roomSubCategoryFlags, BuildCategory buildCategoryFlags, uint surfaceCutoutDDSIndex,
            uint floorCutoutDDSIndex, uint floorCutoutLevelOffset, float floorCutoutBoundsLength,//Version>=0x17
            //float floorCutoutBoundsWidth,//Version>=0x20
            //float floorCutoutOffsetX, float floorCutoutOffsetZ,//Version>=0x21
            IEnumerable<uint> buildableShellDisplayStateHashes,//Version>=0x18
            uint levelBelowOBJDIndex,//Version>=0x19
            uint proxyOBJDIndex,//Version>=0x1b
            uint blueprintXMLIndex,//Version>=0x1d
            uint blueprintIconIndex,//Version>=0x1e
            //float blueprintIconOffsetMinX, float blueprintIconOffsetMinZ, float blueprintIconOffsetMaxX, float blueprintIconOffsetMaxZ,//Version>=0x1f
            SlotPlacement slotPlacementFlags, string surfaceType, string sourceMaterial, Moodlet moodletGiven, int moodletScore,
            uint unknown21, TopicRating[] topicRatings, uint fallbackIndex,
            //uint modularArchEndEastVPXYIndex, uint modularArchEndWestVPXYIndex, uint modularArchConnectVPXYIndex, uint modularArchSingleVPXYIndex,//Version>=0x22
            IEnumerable<TGIBlock> ltgib)
            : this(APIversion, version, materialList,
            instanceName,
            common, objkIndex, objectTypeFlags,
            objectTypeFlags2,
            wallPlacementFlags, movementFlags, cutoutTilesPerLevel, levels, mtDoorList,
            isScriptEnabled, diagonalIndex, ambienceTypeHash, roomCategoryFlags,
            functionCategoryFlags, functionSubCategoryFlags,
            functionSubCategoryFlags2,
            roomSubCategoryFlags, buildCategoryFlags, surfaceCutoutDDSIndex,
            floorCutoutDDSIndex, floorCutoutLevelOffset, floorCutoutBoundsLength,
            0f,//Version>=0x20
            0f, 0f,//Version>=0x21
            buildableShellDisplayStateHashes,
            levelBelowOBJDIndex,
            proxyOBJDIndex,
            blueprintXMLIndex,
            blueprintIconIndex,
            0f, 0f, 0f, 0f,//Version>=0x1f
            slotPlacementFlags, surfaceType, sourceMaterial, moodletGiven, moodletScore,
            unknown21, topicRatings, fallbackIndex,
            0, 0, 0, 0,//Version>=0x22
            ltgib)
        {
            if (checking) if (version > 0x0000001e)
                    throw new InvalidOperationException(String.Format("Constructor requires BlueprintIconOffset bounds for version {0}", version));
        }

        // Version 0x1d
        public ObjectCatalogResource(int APIversion, uint version, IEnumerable<Material> materialList,
            string instanceName,//Version>=0x16
            Common common, uint objkIndex, ObjectType objectTypeFlags,
            ObjectTypeExt objectTypeFlags2,//Version>=0x1a
            WallPlacement wallPlacementFlags, Movement movementFlags, uint cutoutTilesPerLevel, uint levels, IEnumerable<WallCutout> mtDoorList,
            bool isScriptEnabled, uint diagonalIndex, uint ambienceTypeHash, RoomCategory roomCategoryFlags,
            FunctionCategory functionCategoryFlags, FunctionSubCategory functionSubCategoryFlags,
            FunctionSubCategory2 functionSubCategoryFlags2,//Version>=0x1c
            RoomSubCategory roomSubCategoryFlags, BuildCategory buildCategoryFlags, uint surfaceCutoutDDSIndex,
            uint floorCutoutDDSIndex, uint floorCutoutLevelOffset, float floorCutoutBoundsLength,//Version>=0x17
            //float floorCutoutBoundsWidth,//Version>=0x20
            //float floorCutoutOffsetX, float floorCutoutOffsetZ,//Version>=0x21
            IEnumerable<uint> buildableShellDisplayStateHashes,//Version>=0x18
            uint levelBelowOBJDIndex,//Version>=0x19
            uint proxyOBJDIndex,//Version>=0x1b
            uint blueprintXMLIndex,//Version>=0x1d
            //uint blueprintIconIndex,//Version>=0x1e
            //float blueprintIconOffsetMinX, float blueprintIconOffsetMinZ, float blueprintIconOffsetMaxX, float blueprintIconOffsetMaxZ,//Version>=0x1f
            SlotPlacement slotPlacementFlags, string surfaceType, string sourceMaterial, Moodlet moodletGiven, int moodletScore,
            uint unknown21, TopicRating[] topicRatings, uint fallbackIndex,
            //uint modularArchEndEastVPXYIndex, uint modularArchEndWestVPXYIndex, uint modularArchConnectVPXYIndex, uint modularArchSingleVPXYIndex,//Version>=0x22
            IEnumerable<TGIBlock> ltgib)
            : this(APIversion, version, materialList,
            instanceName,
            common, objkIndex, objectTypeFlags,
            objectTypeFlags2,
            wallPlacementFlags, movementFlags, cutoutTilesPerLevel, levels, mtDoorList,
            isScriptEnabled, diagonalIndex, ambienceTypeHash, roomCategoryFlags,
            functionCategoryFlags, functionSubCategoryFlags,
            functionSubCategoryFlags2,
            roomSubCategoryFlags, buildCategoryFlags, surfaceCutoutDDSIndex,
            floorCutoutDDSIndex, floorCutoutLevelOffset, floorCutoutBoundsLength,
            0f,//Version>=0x20
            0f, 0f,//Version>=0x21
            buildableShellDisplayStateHashes,
            levelBelowOBJDIndex,
            proxyOBJDIndex,
            blueprintXMLIndex,
            0,//Version>=0x1e
            0f, 0f, 0f, 0f,//Version>=0x1f
            slotPlacementFlags, surfaceType, sourceMaterial, moodletGiven, moodletScore,
            unknown21, topicRatings, fallbackIndex,
            0, 0, 0, 0,//Version>=0x22
            ltgib)
        {
            if (checking) if (version > 0x0000001d)
                    throw new InvalidOperationException(String.Format("Constructor requires BlueprintIconIndex for version {0}", version));
        }

        // Version 0x1c
        public ObjectCatalogResource(int APIversion, uint version, IEnumerable<Material> materialList,
            string instanceName,//Version>=0x16
            Common common, uint objkIndex, ObjectType objectTypeFlags,
            ObjectTypeExt objectTypeFlags2,//Version>=0x1a
            WallPlacement wallPlacementFlags, Movement movementFlags, uint cutoutTilesPerLevel, uint levels, IEnumerable<WallCutout> mtDoorList,
            bool isScriptEnabled, uint diagonalIndex, uint ambienceTypeHash, RoomCategory roomCategoryFlags,
            FunctionCategory functionCategoryFlags, FunctionSubCategory functionSubCategoryFlags,
            FunctionSubCategory2 functionSubCategoryFlags2,//Version>=0x1c
            RoomSubCategory roomSubCategoryFlags, BuildCategory buildCategoryFlags, uint surfaceCutoutDDSIndex,
            uint floorCutoutDDSIndex, uint floorCutoutLevelOffset, float floorCutoutBoundsLength,//Version>=0x17
            //float floorCutoutBoundsWidth,//Version>=0x20
            //float floorCutoutOffsetX, float floorCutoutOffsetZ,//Version>=0x21
            IEnumerable<uint> buildableShellDisplayStateHashes,//Version>=0x18
            uint levelBelowOBJDIndex,//Version>=0x19
            uint proxyOBJDIndex,//Version>=0x1b
            //uint blueprintXMLIndex,//Version>=0x1d
            //uint blueprintIconIndex,//Version>=0x1e
            //float blueprintIconOffsetMinX, float blueprintIconOffsetMinZ, float blueprintIconOffsetMaxX, float blueprintIconOffsetMaxZ,//Version>=0x1f
            SlotPlacement slotPlacementFlags, string surfaceType, string sourceMaterial, Moodlet moodletGiven, int moodletScore,
            uint unknown21, TopicRating[] topicRatings, uint fallbackIndex,
            //uint modularArchEndEastVPXYIndex, uint modularArchEndWestVPXYIndex, uint modularArchConnectVPXYIndex, uint modularArchSingleVPXYIndex,//Version>=0x22
            IEnumerable<TGIBlock> ltgib)
            : this(APIversion, version, materialList,
            instanceName,
            common, objkIndex, objectTypeFlags,
            objectTypeFlags2,
            wallPlacementFlags, movementFlags, cutoutTilesPerLevel, levels, mtDoorList,
            isScriptEnabled, diagonalIndex, ambienceTypeHash, roomCategoryFlags,
            functionCategoryFlags, functionSubCategoryFlags,
            functionSubCategoryFlags2,
            roomSubCategoryFlags, buildCategoryFlags, surfaceCutoutDDSIndex,
            floorCutoutDDSIndex, floorCutoutLevelOffset, floorCutoutBoundsLength,
            0f,//Version>=0x20
            0f, 0f,//Version>=0x21
            buildableShellDisplayStateHashes,
            levelBelowOBJDIndex,
            proxyOBJDIndex,
            0,//Version>=0x1d
            0,//Version>=0x1e
            0f, 0f, 0f, 0f,//Version>=0x1f
            slotPlacementFlags, surfaceType, sourceMaterial, moodletGiven, moodletScore,
            unknown21, topicRatings, fallbackIndex,
            0, 0, 0, 0,//Version>=0x22
            ltgib)
        {
            if (checking) if (version > 0x0000001c)
                    throw new InvalidOperationException(String.Format("Constructor requires BlueprintIndex for version {0}", version));
        }

        // Version 0x1b
        public ObjectCatalogResource(int APIversion, uint version, IEnumerable<Material> materialList,
            string instanceName,//Version>=0x16
            Common common, uint objkIndex, ObjectType objectTypeFlags,
            ObjectTypeExt objectTypeFlags2,//Version>=0x1a
            WallPlacement wallPlacementFlags, Movement movementFlags, uint cutoutTilesPerLevel, uint levels, IEnumerable<WallCutout> mtDoorList,
            bool isScriptEnabled, uint diagonalIndex, uint ambienceTypeHash, RoomCategory roomCategoryFlags,
            FunctionCategory functionCategoryFlags, FunctionSubCategory functionSubCategoryFlags,
            //FunctionSubCategory2 functionSubCategoryFlags2,//Version>=0x1c
            RoomSubCategory roomSubCategoryFlags, BuildCategory buildCategoryFlags, uint surfaceCutoutDDSIndex,
            uint floorCutoutDDSIndex, uint floorCutoutLevelOffset, float floorCutoutBoundsLength,//Version>=0x17
            //float floorCutoutBoundsWidth,//Version>=0x20
            //float floorCutoutOffsetX, float floorCutoutOffsetZ,//Version>=0x21
            IEnumerable<uint> buildableShellDisplayStateHashes,//Version>=0x18
            uint levelBelowOBJDIndex,//Version>=0x19
            uint proxyOBJDIndex,//Version>=0x1b
            //uint blueprintXMLIndex,//Version>=0x1d
            //uint blueprintIconIndex,//Version>=0x1e
            //float blueprintIconOffsetMinX, float blueprintIconOffsetMinZ, float blueprintIconOffsetMaxX, float blueprintIconOffsetMaxZ,//Version>=0x0x1f
            SlotPlacement slotPlacementFlags, string surfaceType, string sourceMaterial, Moodlet moodletGiven, int moodletScore,
            uint unknown21, TopicRating[] topicRatings, uint fallbackIndex,
            //uint modularArchEndEastVPXYIndex, uint modularArchEndWestVPXYIndex, uint modularArchConnectVPXYIndex, uint modularArchSingleVPXYIndex,//Version>=0x22
            IEnumerable<TGIBlock> ltgib)
            : this(APIversion, version, materialList,
            instanceName,
            common, objkIndex, objectTypeFlags,
            objectTypeFlags2,
            wallPlacementFlags, movementFlags, cutoutTilesPerLevel, levels, mtDoorList,
            isScriptEnabled, diagonalIndex, ambienceTypeHash, roomCategoryFlags,
            functionCategoryFlags, functionSubCategoryFlags,
            0,
            roomSubCategoryFlags, buildCategoryFlags, surfaceCutoutDDSIndex,
            floorCutoutDDSIndex, floorCutoutLevelOffset, floorCutoutBoundsLength,
            0f,//Version>=0x20
            0f, 0f,//Version>=0x21
            buildableShellDisplayStateHashes,
            levelBelowOBJDIndex,
            proxyOBJDIndex,
            0,//Version>=0x1d
            0,//Version>=0x1e
            0f, 0f, 0f, 0f,//Version>=0x1f
            slotPlacementFlags, surfaceType, sourceMaterial, moodletGiven, moodletScore,
            unknown21, topicRatings, fallbackIndex,
            0, 0, 0, 0,//Version>=0x22
            ltgib)
        {
            if (checking) if (version > 0x0000001b)
                    throw new InvalidOperationException(String.Format("Constructor requires FunctionSubCategoryFlags2 for version {0}", version));
        }

        // Version 0x1a
        public ObjectCatalogResource(int APIversion, uint version, IEnumerable<Material> materialList,
            string instanceName,//Version>=0x16
            Common common, uint objkIndex, ObjectType objectTypeFlags,
            ObjectTypeExt objectTypeFlags2,//Version>=0x1a
            WallPlacement wallPlacementFlags, Movement movementFlags, uint cutoutTilesPerLevel, uint levels, IEnumerable<WallCutout> mtDoorList,
            bool isScriptEnabled, uint diagonalIndex, uint ambienceTypeHash, RoomCategory roomCategoryFlags,
            FunctionCategory functionCategoryFlags, FunctionSubCategory functionSubCategoryFlags,
            //FunctionSubCategory2 functionSubCategoryFlags2,//Version>=0x1c
            RoomSubCategory roomSubCategoryFlags, BuildCategory buildCategoryFlags, uint surfaceCutoutDDSIndex,
            uint floorCutoutDDSIndex, uint floorCutoutLevelOffset, float floorCutoutBoundsLength,//Version>=0x17
            //float floorCutoutBoundsWidth,//Version>=0x20
            //float floorCutoutOffsetX, float floorCutoutOffsetZ,//Version>=0x21
            IEnumerable<uint> buildableShellDisplayStateHashes,//Version>=0x18
            uint levelBelowOBJDIndex,//Version>=0x19
            //uint proxyOBJDIndex,//Version>=0x1b
            //uint blueprintXMLIndex,//Version>=0x1d
            //uint blueprintIconIndex,//Version>=0x1e
            //float blueprintIconOffsetMinX, float blueprintIconOffsetMinZ, float blueprintIconOffsetMaxX, float blueprintIconOffsetMaxZ,//Version>=0x1f
            SlotPlacement slotPlacementFlags, string surfaceType, string sourceMaterial, Moodlet moodletGiven, int moodletScore,
            uint unknown21, TopicRating[] topicRatings, uint fallbackIndex,
            //uint modularArchEndEastVPXYIndex, uint modularArchEndWestVPXYIndex, uint modularArchConnectVPXYIndex, uint modularArchSingleVPXYIndex,//Version>=0x22
            IEnumerable<TGIBlock> ltgib)
            : this(APIversion, version, materialList,
            instanceName,
            common, objkIndex, objectTypeFlags,
            objectTypeFlags2,
            wallPlacementFlags, movementFlags, cutoutTilesPerLevel, levels, mtDoorList,
            isScriptEnabled, diagonalIndex, ambienceTypeHash, roomCategoryFlags,
            functionCategoryFlags, functionSubCategoryFlags,
            0,//Version>=1c
            roomSubCategoryFlags, buildCategoryFlags, surfaceCutoutDDSIndex,
            floorCutoutDDSIndex, floorCutoutLevelOffset, floorCutoutBoundsLength,
            0f,//Version>=0x20
            0f, 0f,//Version>=0x21
            buildableShellDisplayStateHashes,
            levelBelowOBJDIndex,
            0,//Version>=0x1b
            0,//Version>=0x1d
            0,//Version>=0x1e
            0f, 0f, 0f, 0f,//Version>=0x1f
            slotPlacementFlags, surfaceType, sourceMaterial, moodletGiven, moodletScore,
            unknown21, topicRatings, fallbackIndex,
            0, 0, 0, 0,//Version>=0x22
            ltgib)
        {
            if (checking) if (version > 0x0000001a)
                    throw new InvalidOperationException(String.Format("Constructor requires ProxyOBJDIndex for version {0}", version));
        }

        // Version 0x19
        public ObjectCatalogResource(int APIversion, uint version, IEnumerable<Material> materialList,
            string instanceName,//Version>=0x16
            Common common, uint objkIndex, ObjectType objectTypeFlags,
            //ObjectTypeExt objectTypeFlags2,//Version>=0x1a
            WallPlacement wallPlacementFlags, Movement movementFlags, uint cutoutTilesPerLevel, uint levels, IEnumerable<WallCutout> mtDoorList,
            bool isScriptEnabled, uint diagonalIndex, uint ambienceTypeHash, RoomCategory roomCategoryFlags,
            FunctionCategory functionCategoryFlags, FunctionSubCategory functionSubCategoryFlags,
            //FunctionSubCategory2 functionSubCategoryFlags2,//Version>=0x1c
            RoomSubCategory roomSubCategoryFlags, BuildCategory buildCategoryFlags, uint surfaceCutoutDDSIndex,
            uint floorCutoutDDSIndex, uint floorCutoutLevelOffset, float floorCutoutBoundsLength,//Version>=0x17
            //float floorCutoutBoundsWidth,//Version>=0x20
            //float floorCutoutOffsetX, float floorCutoutOffsetZ,//Version>=0x21
            IEnumerable<uint> buildableShellDisplayStateHashes,//Version>=0x18
            uint levelBelowOBJDIndex,//Version>=0x19
            //uint proxyOBJDIndex,//Version>=0x1b
            //uint blueprintXMLIndex,//Version>=0x1d
            //uint blueprintIconIndex,//Version>=0x1e
            //float blueprintIconOffsetMinX, float blueprintIconOffsetMinZ, float blueprintIconOffsetMaxX, float blueprintIconOffsetMaxZ,//Version>=0x1f
            SlotPlacement slotPlacementFlags, string surfaceType, string sourceMaterial, Moodlet moodletGiven, int moodletScore,
            uint unknown21, TopicRating[] topicRatings, uint fallbackIndex,
            //uint modularArchEndEastVPXYIndex, uint modularArchEndWestVPXYIndex, uint modularArchConnectVPXYIndex, uint modularArchSingleVPXYIndex,//Version>=0x22
            IEnumerable<TGIBlock> ltgib)
            : this(APIversion, version, materialList,
            instanceName,
            common, objkIndex, objectTypeFlags,
            0,//Version>=0x1a
            wallPlacementFlags, movementFlags, cutoutTilesPerLevel, levels, mtDoorList,
            isScriptEnabled, diagonalIndex, ambienceTypeHash, roomCategoryFlags,
            functionCategoryFlags, functionSubCategoryFlags,
            0,//Version>=1c
            roomSubCategoryFlags, buildCategoryFlags, surfaceCutoutDDSIndex,
            floorCutoutDDSIndex, floorCutoutLevelOffset, floorCutoutBoundsLength,
            0f,//Version>=0x20
            0f, 0f,//Version>=0x21
            buildableShellDisplayStateHashes,
            levelBelowOBJDIndex,
            0,//Version>=0x1b
            0,//Version>=0x1d
            0,//Version>=0x1e
            0f, 0f, 0f, 0f,//Version>=0x1f
            slotPlacementFlags, surfaceType, sourceMaterial, moodletGiven, moodletScore,
            unknown21, topicRatings, fallbackIndex,
            0, 0, 0, 0,//Version>=0x22
            ltgib)
        {
            if (checking) if (version > 0x00000019)
                    throw new InvalidOperationException(String.Format("Constructor requires ObjectTypeFlags2 for version {0}", version));
        }

        // Version 0x18
        public ObjectCatalogResource(int APIversion, uint version, IEnumerable<Material> materialList,
            string instanceName,//Version>=0x16
            Common common, uint objkIndex, ObjectType objectTypeFlags,
            //ObjectTypeExt objectTypeFlags2,//Version>=0x1a
            WallPlacement wallPlacementFlags, Movement movementFlags, uint cutoutTilesPerLevel, uint levels, IEnumerable<WallCutout> mtDoorList,
            bool isScriptEnabled, uint diagonalIndex, uint ambienceTypeHash, RoomCategory roomCategoryFlags,
            FunctionCategory functionCategoryFlags, FunctionSubCategory functionSubCategoryFlags,
            //FunctionSubCategory2 functionSubCategoryFlags2,//Version>=0x1c
            RoomSubCategory roomSubCategoryFlags, BuildCategory buildCategoryFlags, uint surfaceCutoutDDSIndex,
            uint floorCutoutDDSIndex, uint floorCutoutLevelOffset, float floorCutoutBoundsLength,//Version>=0x17
            //float floorCutoutBoundsWidth,//Version>=0x20
            //float floorCutoutOffsetX, float floorCutoutOffsetZ,//Version>=0x21
            IEnumerable<uint> buildableShellDisplayStateHashes,//Version>=0x18
            //uint levelBelowOBJDIndex,//Version>=0x19
            //uint proxyOBJDIndex,//Version>=0x1b
            //uint blueprintXMLIndex,//Version>=0x1d
            //uint blueprintIconIndex,//Version>=0x1e
            //float blueprintIconOffsetMinX, float blueprintIconOffsetMinZ, float blueprintIconOffsetMaxX, float blueprintIconOffsetMaxZ,//Version>=0x1f
            SlotPlacement slotPlacementFlags, string surfaceType, string sourceMaterial, Moodlet moodletGiven, int moodletScore,
            uint unknown21, TopicRating[] topicRatings, uint fallbackIndex,
            //uint modularArchEndEastVPXYIndex, uint modularArchEndWestVPXYIndex, uint modularArchConnectVPXYIndex, uint modularArchSingleVPXYIndex,//Version>=0x22
            IEnumerable<TGIBlock> ltgib)
            : this(APIversion, version, materialList,
            instanceName,
            common, objkIndex, objectTypeFlags,
            0,//Version>=0x1a
            wallPlacementFlags, movementFlags, cutoutTilesPerLevel, levels, mtDoorList,
            isScriptEnabled, diagonalIndex, ambienceTypeHash, roomCategoryFlags,
            functionCategoryFlags, functionSubCategoryFlags,
            0,//Version>=1c
            roomSubCategoryFlags, buildCategoryFlags, surfaceCutoutDDSIndex,
            floorCutoutDDSIndex, floorCutoutLevelOffset, floorCutoutBoundsLength,
            0f,//Version>=0x20
            0f, 0f,//Version>=0x21
            buildableShellDisplayStateHashes,
            0,//Version>=0x19
            0,//Version>=0x1b
            0,//Version>=0x1d
            0,//Version>=0x1e
            0f, 0f, 0f, 0f,//Version>=0x1f
            slotPlacementFlags, surfaceType, sourceMaterial, moodletGiven, moodletScore,
            unknown21, topicRatings, fallbackIndex,
            0, 0, 0, 0,//Version>=0x22
            ltgib)
        {
            if (checking) if (version > 0x00000018)
                    throw new InvalidOperationException(String.Format("Constructor requires LevelBelowOBJDIndex for version {0}", version));
        }

        // Version 0x17
        public ObjectCatalogResource(int APIversion, uint version, IEnumerable<Material> materialList,
            string instanceName,//Version>=0x16
            Common common, uint objkIndex, ObjectType objectTypeFlags,
            //ObjectTypeExt objectTypeFlags2,//Version>=0x1a
            WallPlacement wallPlacementFlags, Movement movementFlags, uint cutoutTilesPerLevel, uint levels, IEnumerable<WallCutout> mtDoorList,
            bool isScriptEnabled, uint diagonalIndex, uint ambienceTypeHash, RoomCategory roomCategoryFlags,
            FunctionCategory functionCategoryFlags, FunctionSubCategory functionSubCategoryFlags,
            //FunctionSubCategory2 functionSubCategoryFlags2,//Version>=0x1c
            RoomSubCategory roomSubCategoryFlags, BuildCategory buildCategoryFlags, uint surfaceCutoutDDSIndex,
            uint floorCutoutDDSIndex, uint floorCutoutLevelOffset, float floorCutoutBoundsLength,//Version>=0x17
            //float floorCutoutBoundsWidth,//Version>=0x20
            //float floorCutoutOffsetX, float floorCutoutOffsetZ,//Version>=0x21
            //IEnumerable<uint> buildableShellDisplayStateHashes,//Version>=0x18
            //uint levelBelowOBJDIndex,//Version>=0x19
            //uint proxyOBJDIndex,//Version>=0x1b
            //uint blueprintXMLIndex,//Version>=0x1d
            //uint blueprintIconIndex,//Version>=0x1e
            //float blueprintIconOffsetMinX, float blueprintIconOffsetMinZ, float blueprintIconOffsetMaxX, float blueprintIconOffsetMaxZ,//Version>=0x1f
            SlotPlacement slotPlacementFlags, string surfaceType, string sourceMaterial, Moodlet moodletGiven, int moodletScore,
            uint unknown21, TopicRating[] topicRatings, uint fallbackIndex,
            //uint modularArchEndEastVPXYIndex, uint modularArchEndWestVPXYIndex, uint modularArchConnectVPXYIndex, uint modularArchSingleVPXYIndex,//Version>=0x22
            IEnumerable<TGIBlock> ltgib)
            : this(APIversion, version, materialList,
            instanceName,
            common, objkIndex, objectTypeFlags,
            0,//Version>=0x1a
            wallPlacementFlags, movementFlags, cutoutTilesPerLevel, levels, mtDoorList,
            isScriptEnabled, diagonalIndex, ambienceTypeHash, roomCategoryFlags,
            functionCategoryFlags, functionSubCategoryFlags,
            0,//Version>=1c
            roomSubCategoryFlags, buildCategoryFlags, surfaceCutoutDDSIndex,
            floorCutoutDDSIndex, floorCutoutLevelOffset, floorCutoutBoundsLength,
            0f,//Version>=0x20
            0f, 0f,//Version>=0x21
            null,//Version>=0x18
            0,//Version>=0x19
            0,//Version>=0x1b
            0,//Version>=0x1d
            0,//Version>=0x1e
            0f, 0f, 0f, 0f,//Version>=0x1f
            slotPlacementFlags, surfaceType, sourceMaterial, moodletGiven, moodletScore,
            unknown21, topicRatings, fallbackIndex,
            0, 0, 0, 0,//Version>=0x22
            ltgib)
        {
            if (checking) if (version > 0x00000017)
                    throw new InvalidOperationException(String.Format("Constructor requires BuildableShellDisplayStateHashes for version {0}", version));
        }

        // Version 0x16
        public ObjectCatalogResource(int APIversion, uint version, IEnumerable<Material> materialList,
            string instanceName,//Version>=0x16
            Common common, uint objkIndex, ObjectType objectTypeFlags,
            //ObjectTypeExt objectTypeFlags2,//Version>=0x1a
            WallPlacement wallPlacementFlags, Movement movementFlags, uint cutoutTilesPerLevel, uint levels, IEnumerable<WallCutout> mtDoorList,
            bool isScriptEnabled, uint diagonalIndex, uint ambienceTypeHash, RoomCategory roomCategoryFlags,
            FunctionCategory functionCategoryFlags, FunctionSubCategory functionSubCategoryFlags,
            //FunctionSubCategory2 functionSubCategoryFlags2,//Version>=0x1c
            RoomSubCategory roomSubCategoryFlags, BuildCategory buildCategoryFlags, uint surfaceCutoutDDSIndex,
            //uint floorCutoutDDSIndex, uint floorCutoutLevelOffset, float floorCutoutBoundsLength,//Version>=0x17
            //float floorCutoutBoundsWidth,//Version>=0x20
            //float floorCutoutOffsetX, float floorCutoutOffsetZ,//Version>=0x21
            //IEnumerable<uint> buildableShellDisplayStateHashes,//Version>=0x18
            //uint levelBelowOBJDIndex,//Version>=0x19
            //uint proxyOBJDIndex,//Version>=0x1b
            //uint blueprintXMLIndex,//Version>=0x1d
            //uint blueprintIconIndex,//Version>=0x1e
            //float blueprintIconOffsetMinX, float blueprintIconOffsetMinZ, float blueprintIconOffsetMaxX, float blueprintIconOffsetMaxZ,//Version>=0x1f
            SlotPlacement slotPlacementFlags, string surfaceType, string sourceMaterial, Moodlet moodletGiven, int moodletScore,
            uint unknown21, TopicRating[] topicRatings, uint fallbackIndex,
            //uint modularArchEndEastVPXYIndex, uint modularArchEndWestVPXYIndex, uint modularArchConnectVPXYIndex, uint modularArchSingleVPXYIndex,//Version>=0x22
            IEnumerable<TGIBlock> ltgib)
            : this(APIversion, version, materialList,
            instanceName,
            common, objkIndex, objectTypeFlags,
            0,//Version>=0x1a
            wallPlacementFlags, movementFlags, cutoutTilesPerLevel, levels, mtDoorList,
            isScriptEnabled, diagonalIndex, ambienceTypeHash, roomCategoryFlags,
            functionCategoryFlags, functionSubCategoryFlags,
            0,//Version>=1c
            roomSubCategoryFlags, buildCategoryFlags, surfaceCutoutDDSIndex,
            0, 0, 0f,//Version>=0x17
            0f,//Version>=0x20
            0f, 0f,//Version>=0x21
            null,//Version>=0x18
            0,//Version>=0x19
            0,//Version>=0x1b
            0,//Version>=0x1d
            0,//Version>=0x1e
            0f, 0f, 0f, 0f,//Version>=0x1f
            slotPlacementFlags, surfaceType, sourceMaterial, moodletGiven, moodletScore,
            unknown21, topicRatings, fallbackIndex,
            0, 0, 0, 0,//Version>=0x22
            ltgib)
        {
            if (checking) if (version > 0x00000016)
                    throw new InvalidOperationException(String.Format("Constructor requires floor cutout details for version {0}", version));
        }

        // Version < 0x16
        public ObjectCatalogResource(int APIversion, uint version, IEnumerable<Material> materialList,
            //string instanceName,//Version>=0x16
            Common common, uint objkIndex, ObjectType objectTypeFlags,
            //ObjectTypeExt objectTypeFlags2,//Version>=0x1a
            WallPlacement wallPlacementFlags, Movement movementFlags, uint cutoutTilesPerLevel, uint levels, IEnumerable<WallCutout> mtDoorList,
            bool isScriptEnabled, uint diagonalIndex, uint ambienceTypeHash, RoomCategory roomCategoryFlags,
            FunctionCategory functionCategoryFlags, FunctionSubCategory functionSubCategoryFlags,
            //FunctionSubCategory2 functionSubCategoryFlags2,//Version>=0x1c
            RoomSubCategory roomSubCategoryFlags, BuildCategory buildCategoryFlags, uint surfaceCutoutDDSIndex,
            //uint floorCutoutDDSIndex, uint floorCutoutLevelOffset, float floorCutoutBoundsLength,//Version>=0x17
            //float floorCutoutBoundsWidth,//Version>=0x20
            //float floorCutoutOffsetX, float floorCutoutOffsetZ,//Version>=0x21
            //IEnumerable<uint> buildableShellDisplayStateHashes,//Version>=0x18
            //uint levelBelowOBJDIndex,//Version>=0x19
            //uint proxyOBJDIndex,//Version>=0x1b
            //uint blueprintXMLIndex,//Version>=0x1d
            //uint blueprintIconIndex,//Version>=0x1e
            //float blueprintIconOffsetMinX, float blueprintIconOffsetMinZ, float blueprintIconOffsetMaxX, float blueprintIconOffsetMaxZ,//Version>=0x1f
            SlotPlacement slotPlacementFlags, string surfaceType, string sourceMaterial, Moodlet moodletGiven, int moodletScore,
            uint unknown21, TopicRating[] topicRatings, uint fallbackIndex,
            //uint modularArchEndEastVPXYIndex, uint modularArchEndWestVPXYIndex, uint modularArchConnectVPXYIndex, uint modularArchSingleVPXYIndex,//Version>=0x22
            IEnumerable<TGIBlock> ltgib)
            : this(APIversion, version, materialList,
            null,//Version>=0x16
            common, objkIndex, objectTypeFlags,
            0,//Version>=0x1a
            wallPlacementFlags, movementFlags, cutoutTilesPerLevel, levels, mtDoorList,
            isScriptEnabled, diagonalIndex, ambienceTypeHash, roomCategoryFlags,
            functionCategoryFlags, functionSubCategoryFlags,
            0,//Version>=1c
            roomSubCategoryFlags, buildCategoryFlags, surfaceCutoutDDSIndex,
            0, 0, 0f,//Version>=0x17
            0f,//Version>=0x20
            0f, 0f,//Version>=0x21
            null,//Version>=0x18
            0,//Version>=0x19
            0,//Version>=0x1b
            0,//Version>=0x1d
            0,//Version>=0x1e
            0f, 0f, 0f, 0f,//Version>=0x1f
            slotPlacementFlags, surfaceType, sourceMaterial, moodletGiven, moodletScore,
            unknown21, topicRatings, fallbackIndex,
            0, 0, 0, 0,//Version>=0x22
            ltgib)
        {
            if (checking) if (version >= 0x00000016)
                    throw new InvalidOperationException(String.Format("Constructor requires InstanceName for version {0}", version));
        }
        #endregion

        #region Data I/O
        protected override void Parse(Stream s)
        {
            BinaryReader r = new BinaryReader(s);

            base.Parse(s);
            this.materialList = new MaterialList(OnResourceChanged, s);
            this.instanceName = (this.version >= 0x00000016) ? BigEndianUnicodeString.Read(s) : null;
            this.common = new Common(requestedApiVersion, OnResourceChanged, s);
            this.objkIndex = r.ReadUInt32();
            this.objectTypeFlags = (ObjectType)r.ReadUInt32();
            if (this.version >= 0x0000001a)
            {
                this.objectTypeFlags2 = (ObjectTypeExt)r.ReadUInt32();
            }
            this.wallPlacementFlags = (WallPlacement)r.ReadUInt32();
            this.movementFlags = (Movement)r.ReadUInt32();
            this.wallCutoutTilesPerLevel = r.ReadUInt32();
            this.levels = r.ReadUInt32();
            this.wallCutoutList = new WallCutoutList(OnResourceChanged, s);
            this.isScriptEnabled = r.ReadByte() != 0;
            this.diagonalIndex = r.ReadUInt32();
            this.ambienceTypeHash = r.ReadUInt32();
            this.roomCategoryFlags = (RoomCategory)r.ReadUInt32();
            this.functionCategoryFlags = (FunctionCategory)r.ReadUInt32();
            this.functionSubCategoryFlags = (FunctionSubCategory)r.ReadUInt64();
            if (this.version >= 0x0000001c)
            {
                this.functionSubCategoryFlags2 = (FunctionSubCategory2)r.ReadUInt64();
            }
            this.roomSubCategoryFlags = (RoomSubCategory)r.ReadUInt64();
            this.buildCategoryFlags = (BuildCategory)r.ReadUInt32();
            this.surfaceCutoutDDSIndex = r.ReadUInt32();
            if (this.version >= 0x00000017)
            {
                this.floorCutoutDDSIndex = r.ReadUInt32();
                this.floorCutoutLevelOffset = r.ReadUInt32();
                this.floorCutoutBoundsLength = r.ReadSingle();
                if (this.version >= 0x00000020)
                {
                    this.floorCutoutBoundsWidth = r.ReadSingle();
                    if (this.version >= 0x00000021)
                    {
                        floorCutoutOffsetX = r.ReadSingle();
                        floorCutoutOffsetZ = r.ReadSingle();
                    }
                }
                if (this.version >= 0x00000018)
                {
                    buildableShellDisplayStateHashes = new UIntList(OnResourceChanged, s);
                    if (this.version >= 0x00000019)
                    {
                        levelBelowOBJDIndex = r.ReadUInt32();
                        if (this.version >= 0x0000001b)
                        {
                            proxyOBJDIndex = r.ReadUInt32();
                            if (this.version >= 0x0000001d)
                            {
                                blueprintXMLIndex = r.ReadUInt32();
                                if (this.version >= 0x0000001e)
                                {
                                    blueprintIconIndex = r.ReadUInt32();
                                    if (this.version >= 0x0000001f)
                                    {
                                        blueprintIconOffsetMinX = r.ReadSingle();
                                        blueprintIconOffsetMinZ = r.ReadSingle();
                                        blueprintIconOffsetMaxX = r.ReadSingle();
                                        blueprintIconOffsetMaxZ = r.ReadSingle();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            this.slotPlacementFlags = (SlotPlacement)r.ReadUInt32();
            this.surfaceType = BigEndianUnicodeString.Read(s);
            this.sourceMaterial = BigEndianUnicodeString.Read(s);
            this.moodletGiven = (Moodlet)r.ReadUInt32();
            this.moodletScore = r.ReadInt32();
            this.unknown21 = r.ReadUInt32();
            for (int i = 0; i < topicRatings.Length; i++)
                topicRatings[i] = new TopicRating(requestedApiVersion, OnResourceChanged, s);
            this.fallbackIndex = r.ReadUInt32();
            if (this.version >= 0x00000022)
            {
                modularArchEndEastVPXYIndex = r.ReadUInt32();
                modularArchEndWestVPXYIndex = r.ReadUInt32();
                modularArchConnectVPXYIndex = r.ReadUInt32();
                modularArchSingleVPXYIndex = r.ReadUInt32();
            }

            list = new TGIBlockList(OnResourceChanged, s, tgiPosn, tgiSize);

            if (checking) if (this.GetType().Equals(typeof(ObjectCatalogResource)) && s.Position != s.Length)
                    throw new InvalidDataException(String.Format("Data stream length 0x{0:X8} is {1:X8} bytes longer than expected at {2:X8}",
                        s.Length, s.Length - s.Position, s.Position));

            wallCutoutList.ParentTGIBlocks = list;
        }

        protected override Stream UnParse()
        {
            Stream s = base.UnParse();
            BinaryWriter w = new BinaryWriter(s);

            if (materialList == null) materialList = new MaterialList(OnResourceChanged);
            materialList.UnParse(s);
            if (this.version >= 0x00000016) BigEndianUnicodeString.Write(s, instanceName);
            if (common == null) common = new Common(requestedApiVersion, OnResourceChanged);
            common.UnParse(s);
            w.Write(objkIndex);
            w.Write((uint)objectTypeFlags);
            if (this.version >= 0x0000001a)
            {
                w.Write((uint)objectTypeFlags2);
            }
            w.Write((uint)wallPlacementFlags);
            w.Write((uint)movementFlags);
            w.Write(wallCutoutTilesPerLevel);
            w.Write(levels);

            if (list == null) list = new TGIBlockList(OnResourceChanged);
            if (wallCutoutList == null) wallCutoutList = new WallCutoutList(OnResourceChanged, list);
            wallCutoutList.UnParse(s);

            w.Write((byte)(isScriptEnabled ? 1 : 0));
            w.Write(diagonalIndex);
            w.Write(ambienceTypeHash);
            w.Write((uint)roomCategoryFlags);
            w.Write((uint)functionCategoryFlags);
            w.Write((ulong)functionSubCategoryFlags);
            if (this.version >= 0x0000001c)
            {
                w.Write((ulong)functionSubCategoryFlags2);
            }
            w.Write((ulong)roomSubCategoryFlags);
            w.Write((uint)buildCategoryFlags);
            w.Write(surfaceCutoutDDSIndex);
            if (this.version >= 0x00000017)
            {
                w.Write(floorCutoutDDSIndex);
                w.Write(floorCutoutLevelOffset);
                w.Write(floorCutoutBoundsLength);
                if (this.version >= 0x00000020)
                {
                    w.Write(floorCutoutBoundsWidth);
                    if (this.version >= 0x00000021)
                    {
                        w.Write(floorCutoutOffsetX);
                        w.Write(floorCutoutOffsetZ);
                    }
                }
                if (this.version >= 0x00000018)
                {
                    if (buildableShellDisplayStateHashes == null) buildableShellDisplayStateHashes = new UIntList(OnResourceChanged);
                    buildableShellDisplayStateHashes.UnParse(s);
                    if (this.version >= 0x00000019)
                    {
                        w.Write(levelBelowOBJDIndex);
                        if (this.version >= 0x0000001b)
                        {
                            w.Write(proxyOBJDIndex);
                            if (this.version >= 0x0000001d)
                            {
                                w.Write(blueprintXMLIndex);
                                if (this.version >= 0x0000001e)
                                {
                                    w.Write(blueprintIconIndex);
                                    if (this.version >= 0x0000001f)
                                    {
                                        w.Write(blueprintIconOffsetMinX);
                                        w.Write(blueprintIconOffsetMinZ);
                                        w.Write(blueprintIconOffsetMaxX);
                                        w.Write(blueprintIconOffsetMaxZ);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            w.Write((uint)slotPlacementFlags);
            BigEndianUnicodeString.Write(s, surfaceType);
            BigEndianUnicodeString.Write(s, sourceMaterial);
            w.Write((uint)moodletGiven);
            w.Write(moodletScore);
            w.Write(unknown21);
            for (int i = 0; i < topicRatings.Length; i++)
            {
                if (topicRatings[i] == null) topicRatings[i] = new TopicRating(0, OnResourceChanged);
                topicRatings[i].UnParse(s);
            }
            w.Write(fallbackIndex);
            if (this.version >= 0x00000022)
            {
                w.Write(modularArchEndEastVPXYIndex);
                w.Write(modularArchEndWestVPXYIndex);
                w.Write(modularArchConnectVPXYIndex);
                w.Write(modularArchSingleVPXYIndex);
            }

            base.UnParse(s);

            w.Flush();

            return s;
        }
        #endregion

        #region AApiVersionedFields
        /// <summary>
        /// The list of available field names on this API object
        /// </summary>
        public override List<string> ContentFields
        {
            get
            {
                List<string> res = base.ContentFields;
                if (this.version < 0x00000022)
                {
                    res.Remove("ModularArchEndEastVPXYIndex");
                    res.Remove("ModularArchEndWestVPXYIndex");
                    res.Remove("ModularArchConnectVPXYIndex");
                    res.Remove("ModularArchSingleVPXYIndex");
                    if (this.version < 0x00000021)
                    {
                        res.Remove("FloorCutoutOffsetX");
                        res.Remove("FloorCutoutOffsetZ");
                        if (this.version < 0x00000020)
                        {
                            res.Remove("FloorCutoutBoundsWidth");
                            if (this.version < 0x0000001f)
                            {
                                res.Remove("BlueprintIconOffsetMinX");
                                res.Remove("BlueprintIconOffsetMinZ");
                                res.Remove("BlueprintIconOffsetMaxX");
                                res.Remove("BlueprintIconOffsetMaxZ");
                                if (this.version < 0x0000001e)
                                {
                                    res.Remove("BlueprintIconIndex");
                                    if (this.version < 0x0000001d)
                                    {
                                        res.Remove("BlueprintXMLIndex");
                                        if (this.version < 0x0000001c)
                                        {
                                            res.Remove("FunctionSubCategoryFlags2");
                                            if (this.version < 0x0000001b)
                                            {
                                                res.Remove("ProxyOBJDIndex");
                                                if (this.version < 0x0000001a)
                                                {
                                                    res.Remove("ObjectTypeFlags2");
                                                    if (this.version < 0x00000019)
                                                    {
                                                        res.Remove("LevelBelowOBJDIndex");
                                                        if (this.version < 0x00000018)
                                                        {
                                                            res.Remove("BuildableShellDisplayStateHashes");
                                                            if (this.version < 0x00000017)
                                                            {
                                                                res.Remove("FloorCutoutDDSIndex");
                                                                res.Remove("FloorCutoutLevelOffset");
                                                                res.Remove("FloorCutoutBoundsLength");
                                                                if (this.version < 0x00000016)
                                                                    res.Remove("InstanceName");
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                return res;
            }
        }
        #endregion

        #region Sub-classes
        [Flags]
        public enum ObjectType : uint
        {
            Unknown00 = 0x00000001,
            AutomaticallyBuyAnotherAfterPlacing = 0x00000002,
            HidesFloorOnPlacement = 0x00000004,
            IsDoor = 0x00000008,

            IsWindow = 0x00000010,
            IsGate = 0x00000020,
            HideWhenWallDown = 0x00000040,
            RabbitHole = 0x00000080,

            IsDiagonal = 0x00000100,
            ForceToFullGrid = 0x00000200,
            RequireFloorAboveIfOutside = 0x00000400,
            IsFireplace = 0x00000800,

            IsChimney = 0x00001000,
            IsFlora = 0x00002000,
            IsColumn = 0x00004000,
            TakeParentAlongWhenPicked = 0x00008000,

            LiveDraggingEnabled = 0x00010000,
            AllowOnSlope = 0x00020000,
            LargeObject = 0x00040000,
            FloatsOnWater = 0x00080000,

            IsGarageDoor = 0x00100000,
            IsMailbox = 0x00200000,
            IgnorePatternSound = 0x00400000,
            IsRoadBridge = 0x00800000,

            AllowWallObjectOnGround = 0x01000000,
            HasFloorCutout = 0x02000000,
            BuildableShell = 0x04000000,
            ElevationFromCeiling = 0x08000000,

            CanDepressTerrain = 0x10000000,
            IgnorePlatformElevation = 0x20000000,
            CantBePlacedOnPlatform = 0x40000000,
            IsShellDoor = 0x80000000,
        }

        [Flags]
        public enum ObjectTypeExt : uint
        {
            SpiralStaircase = 0x00000001,
            CantBePlacedOnDeckOrFoundation = 0x00000002,
            PetCannotSitUnder = 0x00000004,
            PetsCannotJumpOn = 0x00000008,

            LargeAnimalsCannotUse = 0x00000010,
            MustFaceCardinalDirection = 0x00000020,
            IsRug = 0x00000040,
            //Unknown07 = 0x00000080,
            //...
            //Unknown13 = 0x00080000,

            //Unknown14 = 0x00100000,
            //Unknown15 = 0x00200000,
            //Unknown16 = 0x00400000,
            //Unknown17 = 0x00800000,

            //Unknown18 = 0x01000000,
            //Unknown19 = 0x02000000,
            //Unknown1A = 0x04000000,
            //Unknown1B = 0x08000000,

            //Unknown1C = 0x10000000,
            //Unknown1D = 0x20000000,
            //Unknown1E = 0x40000000,
            //Unknown1F = 0x80000000,
        }

        [Flags]
        public enum WallPlacement : uint
        {
            WallAtMinXEdge = 0x00000001,
            WallAtMinZEdge = 0x00000002,
            WallAtMaxXEdge = 0x00000004,
            WallAtMaxZEdge = 0x00000008,

            WallAt01To10Diag = 0x00000010,
            WallAt00To11Diag = 0x00000020,
            NoWallAtMinXEdge = 0x00000040,
            NoWallAtMinZEdge = 0x00000080,

            NoWallAtMaxXEdge = 0x00000100,
            NoWallAtMaxZEdge = 0x00000200,
            NoWallAt01To10Diag = 0x00000400,
            NoWallAt00To11Diag = 0x00000800,

            FlagsApplyToFences = 0x00001000,
            ProhibitsFenceArch = 0x00002000,
            OnWall = 0x00004000,
            IntersectsObjectsOffWall = 0x00008000,

            ApplyCutoutDiagonalShift = 0x00010000,
            CanBeMovedUpDownOnWall = 0x00020000,
            CannotBeMovedUpDownOnWall = 0x00040000,
            //Unknown13 = 0x00080000,

            //Unknown14 = 0x00100000,
            //Unknown15 = 0x00200000,
            //Unknown16 = 0x00400000,
            //Unknown17 = 0x00800000,

            //Unknown18 = 0x01000000,
            //Unknown19 = 0x02000000,
            //Unknown1A = 0x04000000,
            //Unknown1B = 0x08000000,

            //Unknown1C = 0x10000000,
            //Unknown1D = 0x20000000,
            //Unknown1E = 0x40000000,
            //Unknown1F = 0x80000000,
        }

        [Flags]
        public enum Movement : uint
        {
            Unknown00 = 0x00000001,
            StaysAfterEvict = 0x00000002,
            HandToolCannotMoveIt = 0x00000004,
            HandToolCannotDeleteIt = 0x00000008,

            HandToolCannotDuplicateIt = 0x00000010,
            HandToolCanDuplicateWhenHiddenInCatalog = 0x00000020,
            HandToolSkipRecursivePickupTests = 0x00000040,
            GhostsCannotFloatThrough = 0x00000080,

            //Unknown08 = 0x00000100,
        }

        [Flags]
        public enum RoomCategory : uint
        {
            Unused1 = 0x00000001,
            Living = 0x00000002,
            Dining = 0x00000004,
            Kitchen = 0x00000008,

            Nursery = 0x00000010,
            Bathroom = 0x00000020,
            Bedroom = 0x00000040,
            Study = 0x00000080,

            Outside = 0x00000100,
            Community = 0x00000200,
            Residential = 0x00000400,
            Pool = 0x00000800,

            Fountain = 0x00001000,
            ResortLobby = 0x00002000,
            ResortSpa = 0x00004000,
            ResortGym = 0x00008000,

            ResortRestaurant = 0x00010000,
            ResortTikiLounge = 0x00020000,
            ResortArcade = 0x00040000,
            ResortArtGallery = 0x00080000,

            ResortDanceHall = 0x00100000,
            ResortOutdoorPartyArea = 0x00200000,
            ResortPoolArea = 0x00400000,

            Default = 0x80000000,

            //All         = 0xFFFFFFFF,
        }

        [Flags]
        public enum RoomSubCategory : ulong
        {
            //Low DWORD
            Unused1 = 0x00000001,
            Dishwashers = 0x00000002,
            SmallApps = 0x00000004,
            Fridges = 0x00000008,

            Trash = 0x00000010,
            Alarms = 0x00000020,
            Phones = 0x00000040,
            TVs = 0x00000080,

            SmokeAlarm = 0x00000100,
            Unused10 = 0x00000200,
            Audio = 0x00000400,
            Computers = 0x00000800,

            HobbiesSkills = 0x00001000,
            IndoorActivities = 0x00002000,
            LivingChairs = 0x00004000,
            OfficeChairs = 0x00008000,

            Stoves = 0x00010000,
            EatingOut = 0x00020000,
            OutdoorActivities = 0x00040000,
            CeilingLights = 0x00080000,

            FloorLamps = 0x00100000,
            TableLamps = 0x00200000,
            WallLamps = 0x00400000,
            OutdoorLights = 0x00800000,

            Showers = 0x01000000,
            Sinks = 0x02000000,
            Toilets = 0x04000000,
            Tubs = 0x08000000,

            Accents = 0x10000000,
            LawnDecor = 0x20000000,
            WallArtAdult = 0x40000000,
            Plants = 0x80000000,

            //High DWORD
            Mirrors = 0x0000000100000000,
            VideoGames = 0x0000000200000000,
            WallArtKids = 0x0000000400000000,
            Bookshelves = 0x0000000800000000,

            Cabinets = 0x0000001000000000,
            Dressers = 0x0000002000000000,
            DiningChairs = 0x0000004000000000,
            Sofas = 0x0000008000000000,

            OutdoorSeating = 0x0000010000000000,
            RoofDecorations = 0x0000020000000000,
            Beds = 0x0000040000000000,
            BarStools = 0x0000080000000000,

            CoffeeTables = 0x0000100000000000,
            Counters = 0x0000200000000000,
            Desks = 0x0000400000000000,
            EndTables = 0x0000800000000000,

            DiningTables = 0x0001000000000000,
            Furniture = 0x0002000000000000,
            Toys = 0x0004000000000000,
            Transport = 0x0008000000000000,

            Bars = 0x0010000000000000,
            Clocks = 0x0020000000000000,
            WindowDecor = 0x0040000000000000,
            KidsDecor = 0x0080000000000000,

            MiscDecor = 0x0100000000000000,
            Rugs = 0x0200000000000000,
            Laundry = 0x0400000000000000,
            PetEssentials = 0x0800000000000000,

            //Unused61 = 0x1000000000000000,
            //Unused62 = 0x2000000000000000,
            //Unused63 = 0x4000000000000000,
            Default = 0x8000000000000000,
        }

        [Flags]
        public enum FunctionCategory : uint
        {
            //Unused1 = 0x00000001,
            Appliances = 0x00000002,
            Electronics = 0x00000004,
            Entertainment = 0x00000008,

            //Unused5 = 0x00000010,
            Lighting = 0x00000020,
            Plumbing = 0x00000040,
            Decor = 0x00000080,

            Kids = 0x00000100,
            Storage = 0x00000200,
            //Unused11 = 0x00000400,
            Comfort = 0x00000800,

            Surfaces = 0x00001000,
            Vehicles = 0x00002000,
            Pets = 0x00004000,
            ShowStage = 0x00008000,

            Resort = 0x00010000,
            //Unused18 = 0x00020000,
            //Unused19 = 0x00040000,
            //Unused20 = 0x00080000,

            //
            //
            //
            //

            //
            //
            //
            //

            //Unused29 = 0x10000000,
            //Unused30 = 0x20000000,
            Debug = 0x40000000,
            Default = 0x80000000,
        }

        [Flags]
        public enum FunctionSubCategory : ulong
        {
            //Low DWORD
            //Unused1 = 0x00000001,
            AppliancesMisc = 0x00000002,
            AppliancesSmall = 0x00000004,
            AppliancesLarge = 0x00000008,

            DebugTombObjects = 0x00000010,
            DebugFishSpawners = 0x00000020,
            DebugPlantSeedSpawners = 0x00000040,
            ElectronicsTVs = 0x00000080,

            ElectronicsMisc = 0x00000100,
            DebugRockGemMetalSpawners = 0x00000200,
            ElectronicsAudio = 0x00000400,
            ElectronicsComputers = 0x00000800,

            EntertainmentHobbiesSkills = 0x00001000,
            EntertainmentSports = 0x00002000,
            ComfortLivingChairs = 0x00004000,
            PetsHorses = 0x00008000,

            DebugInsectSpawners = 0x00010000,
            EntertainmentParties = 0x00020000,
            EntertainmentMisc = 0x00040000,
            LightingCeiling = 0x00080000,

            LightingFloor = 0x00100000,
            LightingTable = 0x00200000,
            LightingWall = 0x00400000,
            LightingOutdoor = 0x00800000,

            ComfortLoungeChairs = 0x01000000,
            PlumbingSinks = 0x02000000,
            PlumbingToilets = 0x04000000,
            PlumbingShowersAndTubs = 0x08000000,

            DecorMisc = 0x10000000,
            DecorSculptures = 0x20000000,
            DecorWallArt = 0x40000000,
            DecorPlants = 0x80000000,

            //High DWORD
            DecorMirrors = 0x0000000100000000,
            PetsDogs = 0x0000000200000000,
            DebugMisc = 0x0000000400000000,
            StorageBookshelves = 0x0000000800000000,

            SurfacesDisplays = 0x0000001000000000,
            StorageDressers = 0x0000002000000000,
            ComfortDiningChairs = 0x0000004000000000,
            ComfortSofas = 0x0000008000000000,

            ComfortMisc = 0x0000010000000000,
            DecorRoof = 0x0000020000000000,
            ComfortBeds = 0x0000040000000000,
            Unused44 = 0x0000080000000000,

            SurfacesCoffeeTables = 0x0000100000000000,
            SurfacesCounters = 0x0000200000000000,
            SurfacesDesks = 0x0000400000000000,
            SurfacesEndTables = 0x0000800000000000,

            SurfacesDiningTables = 0x0001000000000000,
            KidsFurniture = 0x0002000000000000,
            KidsToys = 0x0004000000000000,
            VehiclesCars = 0x0008000000000000,

            VehiclesBicycles = 0x0010000000000000,
            SurfacesCabinets = 0x0020000000000000,
            DecorWindowDecor = 0x0040000000000000,
            KidsMisc = 0x0080000000000000,

            LightingMisc = 0x0100000000000000,
            PlumbingMisc = 0x0200000000000000,
            StorageMisc = 0x0400000000000000,
            SurfacesMisc = 0x0800000000000000,

            VehiclesMisc = 0x1000000000000000,
            DecorRugs = 0x2000000000000000,
            PetsCats = 0x4000000000000000,
            Default = 0x8000000000000000,
        }
        [Flags]
        public enum FunctionSubCategory2 : ulong
        {
            //Low DWORD
            //Unused1 = 0x00000001,
            FXAndLights = 0x00000002,
            Props = 0x00000004,
            MiscellaneousShowStage = 0x00000008,

            UnderwaterObjects = 0x0000000000000010,
            ResortMisc = 0x0000000000000020,
            Boats = 0x0000000000000040,

            //...
        }

        [Flags]
        public enum BuildCategory : uint
        {
            //Unused1 = 0x00000001,
            Door = 0x00000002,
            Window = 0x00000004,
            Gate = 0x00000008,

            Column = 0x00000010,
            RabbitHole = 0x00000020,
            Fireplace = 0x00000040,
            Chimney = 0x00000080,

            Arch = 0x00000100,
            Flower = 0x00000200,
            Shrub = 0x00000400,
            Tree = 0x00000800,

            Rug = 0x00001000,
            Rock = 0x00002000,
            Shell = 0x00004000,
            Landmark = 0x00008000,


            Elevator = 0x00010000,
            SpiralStaircase = 0x00020000,
            //
            //

            //
            //
            //
            //


            //
            //
            //
            //

            Blueprint = 0x10000000,
            ResortObjects = 0x20000000,
            ModularArch = 0x40000000,
            Default = 0x80000000,
        }

        [Flags]
        public enum SlotPlacement : uint
        {
            //CheckFlags = 0xc3f38, 

            None = 0x01,
            //
            //
            Small = 0x08,

            Medium = 0x10,
            Large = 0x20,
            //
            //

            Sim = 0x0100,
            Chair = 0x0200,
            CounterSink = 0x0400,
            EndTable = 0x0800,

            Stool = 0x1000,
            CounterAppliance = 0x2000,
            //
            //

            //
            //
            Functional = 0x00040000,
            Decorative = 0x00080000,

            //...

            KeepAtSeaLevel = 0x00800000,

            //...

            Upgrade = 0x01000000,
            //MatchFlags    = 0x02000000,
            Vertical = 0x02000000,
            PlacementOnly = 0x04000000,
            //

            CardinalRotation = 0x10000000,
            FullRotation = 0x20000000,
            //RotationFlags = 0x30000000,
            AlwaysUp = 0x40000000,
            //
        }

        public enum Moodlet : uint
        {
            Unused00 = 0x00000000,
            Seating,
            Sleeping,
            Music,
        }

        public enum TopicCategory : uint
        {
            EndOfTopics = 0x00000000,
            Environment,
            Hunger,
            Bladder,

            Energy,
            StressRelief,
            Fun,
            Hygiene,

            Logic,
            Charisma,
            Cooking,
            Athletic,

            Painting,
            Guitar,
            Handiness,
            GroupActivity,


            Upgradable,
            LearnCookingFaster,
            ChildOnly,
            //Unknown13,

            Gardening = 0x00000014,
            Fishing,
            SelfCleaning,
            NeverBreaks,

            Portable,
            Speed,
            Inventing,
            Sculpting,

            PlayPiano,
            PlayBass,
            PlayDrums,
        }

        public class TopicRating : AHandlerElement, IEquatable<TopicRating>, IComparable<TopicRating>
        {
            #region Attributes
            TopicCategory topic;
            int rating;
            #endregion

            #region Constructors
            public TopicRating(int APIversion, EventHandler handler) : base(APIversion, handler) { }
            public TopicRating(int APIversion, EventHandler handler, Stream s) : base(APIversion, handler) { Parse(s); }
            public TopicRating(int APIversion, EventHandler handler, TopicRating basis)
                : this(APIversion, handler, basis.topic, basis.rating) { }
            public TopicRating(int APIversion, EventHandler handler, TopicCategory topic, int rating)
                : base(APIversion, handler)
            {
                this.topic = topic;
                this.rating = rating;
            }
            #endregion

            #region Data I/O
            private void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                this.topic = (TopicCategory)r.ReadUInt32();
                this.rating = r.ReadInt32();
            }
            public void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write((uint)topic);
                w.Write(rating);
            }
            #endregion

            #region IComparable<TopicRating>
            public int CompareTo(TopicRating other)
            {
                int res = topic.CompareTo(other.topic); if (res != 0) return res;
                return rating.CompareTo(other.rating);
            }
            #endregion

            #region IEquatable<TopicRating> Members

            public bool Equals(TopicRating other) { return this.CompareTo(other) == 0; }

            public override bool Equals(object obj)
            {
                return obj as TopicRating != null ? this.Equals((TopicRating)obj) : false;
            }

            public override int GetHashCode()
            {
                return topic.GetHashCode() ^ rating.GetHashCode();
            }

            #endregion

            #region AHandlerElement
            public override List<string> ContentFields { get { return GetContentFields(requestedApiVersion, this.GetType()); } }

            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            #endregion

            #region Content Fields
            [ElementPriority(1)]
            public TopicCategory Topic { get { return topic; } set { if (topic != value) { topic = value; OnElementChanged(); } } }
            [ElementPriority(2)]
            public int Rating { get { return rating; } set { if (rating != value) { rating = value; OnElementChanged(); } } }

            public String Value
            {
                get
                {
                    if (topic == TopicCategory.EndOfTopics)
                        return "--- " + topic + " ---";
                    else
                    {
                        if (rating == 0) return "" + topic;
                        else
                        {
                            if (rating == 11) return "+ " + topic;
                            else return topic + ": " + rating;
                        }
                    }
                }
            }
            #endregion
        }

        public class WallCutout : AHandlerElement, IEquatable<WallCutout>
        {
            public DependentList<TGIBlock> ParentTGIBlocks { get; set; }
            public override List<string> ContentFields { get { List<string> res = GetContentFields(requestedApiVersion, this.GetType()); res.Remove("ParentTGIBlocks"); return res; } }

            #region Attributes
            float leftX;
            float leftZ;
            float rightX;
            float rightZ;
            uint levelOffset;
            uint wallMaskIndex;
            #endregion

            #region Constructors
            public WallCutout(int APIversion, EventHandler handler, DependentList<TGIBlock> ParentTGIBlocks = null)
                : base(APIversion, handler) { this.ParentTGIBlocks = ParentTGIBlocks; }
            public WallCutout(int APIversion, EventHandler handler, Stream s, DependentList<TGIBlock> ParentTGIBlocks = null)
                : this(APIversion, handler, ParentTGIBlocks) { Parse(s); }
            public WallCutout(int APIversion, EventHandler handler, WallCutout basis, DependentList<TGIBlock> ParentTGIBlocks = null)
                : this(APIversion, handler, basis.leftX, basis.leftZ, basis.rightX, basis.rightZ, basis.levelOffset, basis.wallMaskIndex, ParentTGIBlocks ?? basis.ParentTGIBlocks) { }
            public WallCutout(int APIversion, EventHandler handler, float leftX, float leftZ, float rightX, float rightZ, uint levelOffset, uint wallMaskIndex, DependentList<TGIBlock> ParentTGIBlocks = null)
                : this(APIversion, handler, ParentTGIBlocks)
            {
                this.leftX = leftX;
                this.leftZ = leftZ;
                this.rightX = rightX;
                this.rightZ = rightZ;
                this.levelOffset = levelOffset;
                this.wallMaskIndex = wallMaskIndex;
            }
            #endregion

            #region Data I/O
            private void Parse(Stream s)
            {
                BinaryReader r = new BinaryReader(s);
                this.leftX = r.ReadSingle();
                this.leftZ = r.ReadSingle();
                this.rightX = r.ReadSingle();
                this.rightZ = r.ReadSingle();
                this.levelOffset = r.ReadUInt32();
                this.wallMaskIndex = r.ReadUInt32();
            }
            public void UnParse(Stream s)
            {
                BinaryWriter w = new BinaryWriter(s);
                w.Write(leftX);
                w.Write(leftZ);
                w.Write(rightX);
                w.Write(rightZ);
                w.Write(levelOffset);
                w.Write(wallMaskIndex);
            }
            #endregion

            #region IEquatable<MTDoorEntry> Members

            public bool Equals(WallCutout other)
            {
                return true
                    && leftX == other.leftX
                    && leftZ == other.leftZ
                    && rightX == other.rightX
                    && rightZ == other.rightZ
                    && levelOffset == other.levelOffset
                    && wallMaskIndex == other.wallMaskIndex
                    ;
            }

            public override bool Equals(object obj)
            {
                return obj as WallCutout != null ? this.Equals((WallCutout)obj) : false;
            }

            public override int GetHashCode()
            {
                return 0
                    ^ leftX.GetHashCode()
                    ^ leftZ.GetHashCode()
                    ^ rightX.GetHashCode()
                    ^ rightZ.GetHashCode()
                    ^ levelOffset.GetHashCode()
                    ^ wallMaskIndex.GetHashCode()
                    ;
            }

            #endregion

            #region AHandlerElement
            public override int RecommendedApiVersion { get { return recommendedApiVersion; } }
            #endregion

            #region Content Fields
            [ElementPriority(1)]
            public float LeftX { get { return leftX; } set { if (leftX != value) { leftX = value; OnElementChanged(); } } }
            [ElementPriority(2)]
            public float LeftZ { get { return leftZ; } set { if (leftZ != value) { leftZ = value; OnElementChanged(); } } }
            [ElementPriority(3)]
            public float RightX { get { return rightX; } set { if (rightX != value) { rightX = value; OnElementChanged(); } } }
            [ElementPriority(4)]
            public float RightZ { get { return rightZ; } set { if (rightZ != value) { rightZ = value; OnElementChanged(); } } }
            [ElementPriority(5)]
            public uint LevelOffset { get { return levelOffset; } set { if (levelOffset != value) { levelOffset = value; OnElementChanged(); } } }
            [ElementPriority(6), TGIBlockListContentField("ParentTGIBlocks")]
            public uint WallMaskIndex { get { return wallMaskIndex; } set { if (wallMaskIndex != value) { wallMaskIndex = value; OnElementChanged(); } } }

            public String Value { get { return ValueBuilder; } }
            #endregion
        }

        public class WallCutoutList : DependentList<WallCutout>
        {
            private DependentList<TGIBlock> _ParentTGIBlocks;
            public DependentList<TGIBlock> ParentTGIBlocks
            {
                get { return _ParentTGIBlocks; }
                set { if (_ParentTGIBlocks != value) { _ParentTGIBlocks = value; foreach (var i in this) i.ParentTGIBlocks = _ParentTGIBlocks; } }
            }
            //public override List<string> ContentFields { get { List<string> res = GetContentFields(0, this.GetType()); res.Remove("ParentTGIBlocks"); return res; } }

            #region Constructors
            public WallCutoutList(EventHandler handler, DependentList<TGIBlock> ParentTGIBlocks = null) : base(handler, Byte.MaxValue) { _ParentTGIBlocks = ParentTGIBlocks; }
            public WallCutoutList(EventHandler handler, Stream s, DependentList<TGIBlock> ParentTGIBlocks = null)
                : this(null, ParentTGIBlocks) { elementHandler = handler; Parse(s); this.handler = handler; }
            public WallCutoutList(EventHandler handler, IEnumerable<WallCutout> mtDoorList, DependentList<TGIBlock> ParentTGIBlocks = null)
                : this(null, ParentTGIBlocks) { elementHandler = handler; foreach (var t in mtDoorList) this.Add((WallCutout)t.Clone(null)); this.handler = handler; }
            #endregion

            #region Data I/O
            protected override int ReadCount(Stream s) { return (new BinaryReader(s)).ReadByte(); }
            protected override WallCutout CreateElement(Stream s) { return new WallCutout(0, elementHandler, s); }
            protected override void WriteCount(Stream s, int count) { (new BinaryWriter(s)).Write((byte)count); }
            protected override void WriteElement(Stream s, WallCutout element) { element.UnParse(s); }
            #endregion

            public override void Add() { this.Add(new WallCutout(0, handler, _ParentTGIBlocks)); }
            public override void Add(WallCutout item) { item.ParentTGIBlocks = _ParentTGIBlocks; base.Add(item); }
        }
        #endregion

        #region Content Fields
        //--insert Version: ElementPriority(1)
        [ElementPriority(12)]
        public MaterialList Materials { get { return materialList; } set { if (materialList != value) { materialList = value == null ? null : new MaterialList(OnResourceChanged, value); } OnResourceChanged(this, EventArgs.Empty); } }
        [ElementPriority(13)]
        public string InstanceName
        {
            get { if (version < 0x00000016) throw new InvalidOperationException(); return instanceName; }
            set { if (version < 0x00000016) throw new InvalidOperationException(); if (instanceName != value) { instanceName = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        //--insert CommonBlock: ElementPriority(11)
        [ElementPriority(21), TGIBlockListContentField("TGIBlocks")]
        public uint OBJKIndex { get { return objkIndex; } set { if (objkIndex != value) { objkIndex = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(22)]
        public ObjectType ObjectTypeFlags { get { return objectTypeFlags; } set { if (objectTypeFlags != value) { objectTypeFlags = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(23)]
        public ObjectTypeExt ObjectTypeFlags2
        {
            get { if (version < 0x0000001a) throw new InvalidOperationException(); return objectTypeFlags2; }
            set { if (version < 0x0000001a) throw new InvalidOperationException(); if (objectTypeFlags2 != value) { objectTypeFlags2 = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(24)]
        public WallPlacement WallPlacementFlags { get { return wallPlacementFlags; } set { if (wallPlacementFlags != value) { wallPlacementFlags = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(25)]
        public Movement MovementFlags { get { return movementFlags; } set { if (movementFlags != value) { movementFlags = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(26)]
        public uint CutoutTilesPerLevel { get { return wallCutoutTilesPerLevel; } set { if (wallCutoutTilesPerLevel != value) { wallCutoutTilesPerLevel = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(27)]
        public uint Levels { get { return levels; } set { if (levels != value) { levels = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(28)]
        public WallCutoutList WallCutouts { get { return wallCutoutList; } set { if (!wallCutoutList.Equals(value)) { wallCutoutList = value == null ? null : new WallCutoutList(OnResourceChanged, value, list); OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(29)]
        public bool IsScriptEnabled { get { return isScriptEnabled; } set { if (IsScriptEnabled != value) { isScriptEnabled = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(30), TGIBlockListContentField("TGIBlocks")]
        public uint DiagonalIndex { get { return diagonalIndex; } set { if (diagonalIndex != value) { diagonalIndex = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(31)]
        public uint AmbienceTypeHash { get { return ambienceTypeHash; } set { if (ambienceTypeHash != value) { ambienceTypeHash = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(32)]
        public RoomCategory RoomCategoryFlags { get { return roomCategoryFlags; } set { if (roomCategoryFlags != value) { roomCategoryFlags = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(33)]
        public FunctionCategory FunctionCategoryFlags { get { return functionCategoryFlags; } set { if (functionCategoryFlags != value) { functionCategoryFlags = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(34)]
        public FunctionSubCategory FunctionSubCategoryFlags { get { return functionSubCategoryFlags; } set { if (functionSubCategoryFlags != value) { functionSubCategoryFlags = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(35)]
        public FunctionSubCategory2 FunctionSubCategoryFlags2
        {
            get { if (version < 0x0000001c) throw new InvalidOperationException(); return functionSubCategoryFlags2; }
            set { if (version < 0x0000001c) throw new InvalidOperationException(); if (functionSubCategoryFlags2 != value) { functionSubCategoryFlags2 = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(36)]
        public RoomSubCategory RoomSubCategoryFlags { get { return roomSubCategoryFlags; } set { if (roomSubCategoryFlags != value) { roomSubCategoryFlags = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(37)]
        public BuildCategory BuildCategoryFlags { get { return buildCategoryFlags; } set { if (buildCategoryFlags != value) { buildCategoryFlags = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(38), TGIBlockListContentField("TGIBlocks")]
        public uint SurfaceCutoutDDSIndex { get { return surfaceCutoutDDSIndex; } set { if (surfaceCutoutDDSIndex != value) { surfaceCutoutDDSIndex = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(39), TGIBlockListContentField("TGIBlocks")]
        public uint FloorCutoutDDSIndex
        {
            get { if (version < 0x00000017) throw new InvalidOperationException(); return floorCutoutDDSIndex; }
            set { if (version < 0x00000017) throw new InvalidOperationException(); if (floorCutoutDDSIndex != value) { floorCutoutDDSIndex = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(40)]
        public uint FloorCutoutLevelOffset
        {
            get { if (version < 0x00000017) throw new InvalidOperationException(); return floorCutoutLevelOffset; }
            set { if (version < 0x00000017) throw new InvalidOperationException(); if (floorCutoutLevelOffset != value) { floorCutoutLevelOffset = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(41)]
        public float FloorCutoutBoundsLength
        {
            get { if (version < 0x00000017) throw new InvalidOperationException(); return floorCutoutBoundsLength; }
            set { if (version < 0x00000017) throw new InvalidOperationException(); if (floorCutoutBoundsLength != value) { floorCutoutBoundsLength = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(42)]
        public float FloorCutoutBoundsWidth
        {
            get { if (version < 0x00000020) throw new InvalidOperationException(); return floorCutoutBoundsWidth; }
            set { if (version < 0x00000020) throw new InvalidOperationException(); if (floorCutoutBoundsWidth != value) { floorCutoutBoundsWidth = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(43)]
        public float FloorCutoutOffsetX
        {
            get { if (version < 0x00000021) throw new InvalidOperationException(); return floorCutoutOffsetX; }
            set { if (version < 0x00000021) throw new InvalidOperationException(); if (floorCutoutOffsetX != value) { floorCutoutOffsetX = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(44)]
        public float FloorCutoutOffsetZ
        {
            get { if (version < 0x00000021) throw new InvalidOperationException(); return floorCutoutOffsetZ; }
            set { if (version < 0x00000021) throw new InvalidOperationException(); if (floorCutoutOffsetZ != value) { floorCutoutOffsetZ = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(45)]
        public UIntList BuildableShellDisplayStateHashes
        {
            get { if (version < 0x00000018) throw new InvalidOperationException(); return buildableShellDisplayStateHashes; }
            set { if (version < 0x00000018) throw new InvalidOperationException(); if (buildableShellDisplayStateHashes != value) { buildableShellDisplayStateHashes = value == null ? null : new UIntList(OnResourceChanged, value); } OnResourceChanged(this, EventArgs.Empty); }
        }
        [ElementPriority(46), TGIBlockListContentField("TGIBlocks")]
        public uint LevelBelowOBJDIndex
        {
            get { if (version < 0x00000019) throw new InvalidOperationException(); return levelBelowOBJDIndex; }
            set { if (version < 0x00000019) throw new InvalidOperationException(); if (levelBelowOBJDIndex != value) { levelBelowOBJDIndex = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(47), TGIBlockListContentField("TGIBlocks")]
        public uint ProxyOBJDIndex
        {
            get { if (version < 0x0000001b) throw new InvalidOperationException(); return proxyOBJDIndex; }
            set { if (version < 0x0000001b) throw new InvalidOperationException(); if (proxyOBJDIndex != value) { proxyOBJDIndex = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(48), TGIBlockListContentField("TGIBlocks")]
        public uint BlueprintXMLIndex
        {
            get { if (version < 0x0000001d) throw new InvalidOperationException(); return blueprintXMLIndex; }
            set { if (version < 0x0000001d) throw new InvalidOperationException(); if (blueprintXMLIndex != value) { blueprintXMLIndex = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(49), TGIBlockListContentField("TGIBlocks")]
        public uint BlueprintIconIndex
        {
            get { if (version < 0x0000001e) throw new InvalidOperationException(); return blueprintIconIndex; }
            set { if (version < 0x0000001e) throw new InvalidOperationException(); if (blueprintIconIndex != value) { blueprintIconIndex = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(50)]
        public float BlueprintIconOffsetMinX
        {
            get { if (version < 0x0000001f) throw new InvalidOperationException(); return blueprintIconOffsetMinX; }
            set { if (version < 0x0000001f) throw new InvalidOperationException(); if (blueprintIconOffsetMinX != value) { blueprintIconOffsetMinX = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(51)]
        public float BlueprintIconOffsetMinZ
        {
            get { if (version < 0x0000001f) throw new InvalidOperationException(); return blueprintIconOffsetMinZ; }
            set { if (version < 0x0000001f) throw new InvalidOperationException(); if (blueprintIconOffsetMinZ != value) { blueprintIconOffsetMinZ = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(52)]
        public float BlueprintIconOffsetMaxX
        {
            get { if (version < 0x0000001f) throw new InvalidOperationException(); return blueprintIconOffsetMaxX; }
            set { if (version < 0x0000001f) throw new InvalidOperationException(); if (blueprintIconOffsetMaxX != value) { blueprintIconOffsetMaxX = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(53)]
        public float BlueprintIconOffsetMaxZ
        {
            get { if (version < 0x0000001f) throw new InvalidOperationException(); return blueprintIconOffsetMaxZ; }
            set { if (version < 0x0000001f) throw new InvalidOperationException(); if (blueprintIconOffsetMaxZ != value) { blueprintIconOffsetMaxZ = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(54)]
        public SlotPlacement SlotPlacementFlags { get { return slotPlacementFlags; } set { if (slotPlacementFlags != value) { slotPlacementFlags = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(55)]
        public string SurfaceType { get { return surfaceType; } set { if (surfaceType != value) { surfaceType = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(56)]
        public string SourceMaterial { get { return sourceMaterial; } set { if (sourceMaterial != value) { sourceMaterial = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(57)]
        public Moodlet MoodletGiven { get { return moodletGiven; } set { if (moodletGiven != value) { moodletGiven = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(58)]
        public int MoodletScore { get { return moodletScore; } set { if (moodletScore != value) { moodletScore = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(59)]
        public uint Unknown21 { get { return unknown21; } set { if (unknown21 != value) { unknown21 = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(60)]
        public TopicRating[] TopicRatings
        {
            get { return topicRatings; }
            set
            {
                if (value.Length != this.topicRatings.Length) throw new ArgumentLengthException("TopicRatings", this.topicRatings.Length);
                if (!topicRatings.Equals<TopicRating>(value)) { topicRatings = value == null ? null : (TopicRating[])value.Clone(); OnResourceChanged(this, EventArgs.Empty); }
            }
        }
        [ElementPriority(61), TGIBlockListContentField("TGIBlocks")]
        public uint FallbackIndex { get { return fallbackIndex; } set { if (fallbackIndex != value) { fallbackIndex = value; OnResourceChanged(this, EventArgs.Empty); } } }
        [ElementPriority(62), TGIBlockListContentField("TGIBlocks")]
        public uint ModularArchEndEastVPXYIndex
        {
            get { if (version < 0x00000022) throw new InvalidOperationException(); return modularArchEndEastVPXYIndex; }
            set { if (version < 0x00000022) throw new InvalidOperationException(); if (modularArchEndEastVPXYIndex != value) { modularArchEndEastVPXYIndex = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(63), TGIBlockListContentField("TGIBlocks")]
        public uint ModularArchEndWestVPXYIndex
        {
            get { if (version < 0x00000022) throw new InvalidOperationException(); return modularArchEndWestVPXYIndex; }
            set { if (version < 0x00000022) throw new InvalidOperationException(); if (modularArchEndWestVPXYIndex != value) { modularArchEndWestVPXYIndex = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(64), TGIBlockListContentField("TGIBlocks")]
        public uint ModularArchConnectVPXYIndex
        {
            get { if (version < 0x00000022) throw new InvalidOperationException(); return modularArchConnectVPXYIndex; }
            set { if (version < 0x00000022) throw new InvalidOperationException(); if (modularArchConnectVPXYIndex != value) { modularArchConnectVPXYIndex = value; OnResourceChanged(this, EventArgs.Empty); } }
        }
        [ElementPriority(65), TGIBlockListContentField("TGIBlocks")]
        public uint ModularArchSingleVPXYIndex
        {
            get { if (version < 0x00000022) throw new InvalidOperationException(); return modularArchSingleVPXYIndex; }
            set { if (version < 0x00000022) throw new InvalidOperationException(); if (modularArchSingleVPXYIndex != value) { modularArchSingleVPXYIndex = value; OnResourceChanged(this, EventArgs.Empty); } }
        }

        public override TGIBlockList TGIBlocks { get { return list; } set { if (list != value) { list = new TGIBlockList(OnResourceChanged, value); OnResourceChanged(this, EventArgs.Empty); wallCutoutList.ParentTGIBlocks = list; } } }
        #endregion
    }
}
