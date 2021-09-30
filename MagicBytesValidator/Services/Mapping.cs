﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MagicBytesValidator.Exceptions;
using MagicBytesValidator.Formats;
using MagicBytesValidator.Models;

namespace MagicBytesValidator.Services
{
    /// <inheritdoc />
    public class Mapping : IMapping
    {
        /// <inheritdoc />
        public IReadOnlyList<FileType> FileTypes => _fileTypes;

        private readonly List<FileType> _fileTypes = FileTypeCollector.CollectFileTypes().ToList();

        /// <inheritdoc />
        public FileType? FindByMimeType(string mimeType)
        {
            if (string.IsNullOrEmpty(mimeType))
            {
                throw new ArgumentEmptyException(nameof(mimeType));
            }

            return _fileTypes.FirstOrDefault(f =>
                                                 string.Equals(f.MimeType, mimeType,
                                                               StringComparison.InvariantCultureIgnoreCase)
                                            );
        }

        /// <inheritdoc />
        public FileType? FindByExtension(string extension)
        {
            if (string.IsNullOrEmpty(extension))
            {
                throw new ArgumentEmptyException(nameof(extension));
            }

            return _fileTypes.FirstOrDefault(
                                             f => f.Extensions.Any(fe => string.Equals(fe, extension,
                                                                    StringComparison.InvariantCultureIgnoreCase))
                                            );
        }

        /// <inheritdoc />
        public void Register(FileType fileType)
        {
            _fileTypes.Add(fileType);
        }

        public void Register(IReadOnlyList<FileType> fileTypes)
        {
            if (!fileTypes.Any())
            {
                return;
            }

            _fileTypes.AddRange(fileTypes);
        }

        public void Register(Assembly assembly)
        {
            var fileTypes = FileTypeCollector.CollectFileTypes(assembly).ToList();

            _fileTypes.AddRange(fileTypes);
        }

        /// <summary>
        /// Tries to find a known FileType by magic byte sequence.
        /// </summary>
        /// <param name="magicByteSequence">Magic byte sequence that should be searched for</param>
        /// <returns>FileType that contains the given magic byte sequence</returns>
        /// <exception cref="ArgumentEmptyException">When magicByteSequence is empty</exception>
        private FileType? FindByMagicByteSequence(byte[] magicByteSequence)
        {
            if (!magicByteSequence.Any())
            {
                throw new ArgumentEmptyException(nameof(magicByteSequence));
            }

            return _fileTypes.FirstOrDefault(
                                             f => f.MagicByteSequences.Any(mb => mb.SequenceEqual(magicByteSequence))
                                            );
        }
    }
}