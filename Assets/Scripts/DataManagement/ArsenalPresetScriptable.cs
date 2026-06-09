using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Arsenal
{
    [Serializable]
    public class BlockData
    {
        public string name;
        public string imageAddressableKey;
        public string prefabAddressableKey;
        public bool isSelected;
        public float[] probabilitiesByTurn = new float[] { 0.1f, 0.2f, 0.4f, 0.7f, 1f };

        public BlockData() { }

        public BlockData(ArsenalBlock block)
        {
            name = block.Name; imageAddressableKey = block.ImageAddressable; prefabAddressableKey = block.PrefabAddressable;
            isSelected = block.IsSelected; probabilitiesByTurn = block.ProbabilitiesByTurn;
        }
    }
    [Serializable]
    public class ArsenalBlock : Block
    {
        [SerializeField]
        private bool _isSelected = false;
        [SerializeField]
        private float[] _probabilitiesByTurn = new float[] { 0.1f, 0.2f, 0.4f, 0.7f, 1f };

        public float[] ProbabilitiesByTurn => _probabilitiesByTurn;
        public bool IsSelected { get { return _isSelected; } set { _isSelected = value; } }

        public ArsenalBlock(BlockData data) : base(data.name, data.imageAddressableKey, data.prefabAddressableKey)
        {
            _isSelected = data.isSelected; _probabilitiesByTurn = data.probabilitiesByTurn;
        }
    }

    [CreateAssetMenu(fileName = "ArsenalPresetScriptable", menuName = "Scriptable Objects/ArsenalPresetScriptable")]
    public class ArsenalPresetScriptable : AbstractPresetScriptable<ArsenalBlock>
    {
        [Serializable]
        private class ArrayWrapper<T> {
            [SerializeField] private T[] _array;
            public T[] Array => _array;
            public ArrayWrapper(T[] arr) => _array = arr;
            public int Length => Array.Length;
        }

        [SerializeField]
        private ArsenalBlock[] _arsenalBlocks;

        public override IEnumerable<ArsenalBlock> Blocks => _arsenalBlocks;
        public void BuildAlias()
        {
            var selected = _arsenalBlocks.Where(b => b.IsSelected).ToArray();
            if (selected.Length == 0)
            {
                _prob = new float[0];
                _alias = new int[0];
                return;
            }
            float[] weights = new float[selected.Length];
            for (int i = 0; i < selected.Length; i++)
                weights[i] = 1f; 
            _prob = new float[weights.Length];
            _alias = new int[weights.Length];
            unsafe { BuildAliasTable(weights, _prob, _alias); }
        }
        public override ArsenalBlock NextBlock()
        {
            if (_prob == null || _prob.Length == 0)
                BuildAlias(); 
            int column = UnityEngine.Random.Range(0, _prob.Length);
            int index = UnityEngine.Random.value < _prob[column] ? column : _alias[column];
            return _arsenalBlocks.Where(b => b.IsSelected).ElementAt(index);
        }

        #region Json Load and Save
        public void LoadArsenalPreset()
        {
            string path = Path.Combine(Application.persistentDataPath, MConfig.ARSENAL_SAVE_FILE_NAME);
            if (!File.Exists(path)) SavePresetAsJson();
            string json = File.ReadAllText(path);
            Debug.Log(json);
            ArrayWrapper<BlockData> arsenal = JsonUtility.FromJson<ArrayWrapper<BlockData>>(json);
            Debug.Log(arsenal.Array.Length);
            _arsenalBlocks = new ArsenalBlock[arsenal.Length];
            for (int i = 0; i < arsenal.Length; i++)
                _arsenalBlocks[i] = new ArsenalBlock(arsenal.Array[i]);
        }

        public void SavePresetAsJson()
        {
            string path = Path.Combine(Application.persistentDataPath, MConfig.ARSENAL_SAVE_FILE_NAME);
            BlockData[] blocks = new BlockData[_arsenalBlocks.Length];
            for (int i = 0; i < _arsenalBlocks.Length; i++)
                blocks[i] = new BlockData(_arsenalBlocks[i]);
            File.WriteAllText(path, JsonUtility.ToJson(new ArrayWrapper<BlockData>(blocks)));
        }
        #endregion
        #region Binary Load and Save
        public void LoadPresetBinary()
        {
            string path = Path.Combine(Application.persistentDataPath, "Arsenal.bin");
            if (!File.Exists(path))
            {
                SavePresetAsBinary();
                return;
            }
            using var stream = new FileStream(path, FileMode.Open);
            using var reader = new BinaryReader(stream);
            int count = reader.ReadInt32();
            _arsenalBlocks = new ArsenalBlock[count];
            for (int i = 0; i < count; i++)
            {
                string name = reader.ReadString();
                string imageAddr = reader.ReadString();
                string prefabAddr = reader.ReadString();
                bool isSelected = reader.ReadBoolean();
                int probCount = reader.ReadInt32();
                float[] probs = new float[probCount];
                if (probCount > 0)
                {
                    unsafe
                    {
                        byte* floatBuf = stackalloc byte[probCount * 4];
                        stream.Read(new Span<byte>(floatBuf, probCount * 4));
                        for (int j = 0; j < probCount; j++)
                            probs[j] = *(float*)(floatBuf + j * 4);
                    }
                }
                BlockData data = new BlockData
                {
                    name = name,
                    imageAddressableKey = imageAddr,
                    prefabAddressableKey = prefabAddr,
                    isSelected = isSelected,
                    probabilitiesByTurn = probs
                };
                _arsenalBlocks[i] = new ArsenalBlock(data);
            }
        }
        public void SavePresetAsBinary()
        {
            string path = Path.Combine(Application.persistentDataPath, "Arsenal.bin");
            using var stream = new FileStream(path, FileMode.Create);
            using var writer = new BinaryWriter(stream);
            writer.Write(_arsenalBlocks.Length);
            foreach (var block in _arsenalBlocks)
            {
                writer.Write(block.Name ?? string.Empty);
                writer.Write(block.ImageAddressable ?? string.Empty);
                writer.Write(block.PrefabAddressable ?? string.Empty);
                writer.Write(block.IsSelected);
                writer.Write(block.ProbabilitiesByTurn.Length);
                foreach (float p in block.ProbabilitiesByTurn)
                    writer.Write(p);
            }
        }
        private static unsafe string ReadStringUnsafe(Stream stream)
        {
            byte[] lenBuf = new byte[4];
            stream.Read(lenBuf, 0, 4);
            int length = BitConverter.ToInt32(lenBuf, 0);
            if (length == 0) return string.Empty;
            char* chars = stackalloc char[length];
            int bytesRead = stream.Read(new Span<byte>((byte*)chars, length * 2));
            if (bytesRead != length * 2)
                throw new EndOfStreamException("Unexpected end of stream reading string.");
            return new string(chars, 0, length);
        }
        private static unsafe void WriteString(BinaryWriter writer, string s)
        {
            writer.Write(s?.Length ?? 0);
            if (s == null) return;
            fixed (char* ptr = s)
            {
                for (int i = 0; i < s.Length; i++)
                    writer.Write(ptr[i]); 
            }
        }
        #endregion
    }
}