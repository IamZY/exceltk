﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Security.Cryptography;

namespace ICSharpCode.SharpZipLib.Encryption {
    public static class Extension {
        public static KeySizes[] CloneKeySizesArray(this KeySizes[] src) {
            return (KeySizes[])(src.Clone());
        }
    }
    public abstract class Rijndael2 : SymmetricAlgorithm {
        #if OS_WINDOWS
        public static new Rijndael2 Create() {
        #else
        public static new Rijndael2 Create() {
        #endif
            return new RijndaelImplementation();
        }

        protected Rijndael2() {
            LegalBlockSizesValue=s_legalBlockSizes.CloneKeySizesArray();
            LegalKeySizesValue=s_legalKeySizes.CloneKeySizesArray();
            KeySizeValue=256;
            BlockSizeValue=128;
        }

        private static readonly KeySizes[] s_legalBlockSizes=
        {
            new KeySizes(minSize: 128, maxSize: 256, skipSize: 64)
        };

        private static readonly KeySizes[] s_legalKeySizes=
        {
            new KeySizes(minSize: 128, maxSize: 256, skipSize: 64)
        };
    }

    /// <summary>
    /// Internal implementation of Rijndael.
    /// This class is returned from Rijndael.Create() instead of the public RijndaelManaged to 
    /// be consistent with the rest of the static Create() methods which return opaque types.
    /// They both have have the same implementation.
    /// </summary>
    internal sealed class RijndaelImplementation : Rijndael2 {
        private readonly Aes _impl;

        internal RijndaelImplementation() {
            LegalBlockSizesValue=new KeySizes[] { new KeySizes(minSize: 128, maxSize: 128, skipSize: 0) };

            // This class wraps Aes
            _impl=Aes.Create();
        }

        public override int BlockSize {
            get {
                return _impl.BlockSize;
            }
            set {
                // Values which were legal in desktop RijndaelManaged but not here in this wrapper type
                if (value==192||value==256)
                    throw new PlatformNotSupportedException("Cryptography_Rijndael_BlockSize Can NOT be 192 or 256");

                // Any other invalid block size will get the normal “invalid block size” exception.
                _impl.BlockSize=value;
            }
        }

        public override byte[] IV {
            get {
                return _impl.IV;
            }
            set {
                _impl.IV=value;
            }
        }

        public override byte[] Key {
            get {
                return _impl.Key;
            }
            set {
                _impl.Key=value;
            }
        }

        public override int KeySize {
            get {
                return _impl.KeySize;
            }
            set {
                _impl.KeySize=value;
            }
        }
        public override CipherMode Mode {
            get {
                return _impl.Mode;
            }
            set {
                _impl.Mode=value;
            }
        }

        public override PaddingMode Padding {
            get {
                return _impl.Padding;
            }
            set {
                _impl.Padding=value;
            }
        }

        public override KeySizes[] LegalKeySizes {
            get {
                return _impl.LegalKeySizes;
            }
        }
        public override ICryptoTransform CreateEncryptor() {
            return _impl.CreateEncryptor();
        }
        public override ICryptoTransform CreateEncryptor(byte[] rgbKey, byte[] rgbIV) {
            return _impl.CreateEncryptor(rgbKey, rgbIV);
        }
        public override ICryptoTransform CreateDecryptor() {
            return _impl.CreateDecryptor();
        }
        public override ICryptoTransform CreateDecryptor(byte[] rgbKey, byte[] rgbIV) {
            return _impl.CreateDecryptor(rgbKey, rgbIV);
        }
        public override void GenerateIV() {
            _impl.GenerateIV();
        }
        public override void GenerateKey() {
            _impl.GenerateKey();
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
#if !OS_WINDOWS
                _impl.Dispose();
#else
                _impl.Clear();
#endif
            }
        }
    }
}