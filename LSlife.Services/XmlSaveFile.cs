using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace LSlife;

internal static class XmlSaveFile
{
    private static readonly object WriteLock = new object();

    internal static XDocument Load(string path)
    {
        try
        {
            return XDocument.Load(path);
        }
        catch (Exception error) when (error is IOException || error is UnauthorizedAccessException || error is XmlException)
        {
            throw new IOException($"Impossible de lire le XML « {path} ». Copie de secours éventuelle : « {path}.bak ».", error);
        }
    }

    internal static void Save(XDocument document, string path)
    {
        string fullPath = Path.GetFullPath(path);
        string backupPath = fullPath + ".bak";
        string temporaryPath = fullPath + "." + Guid.NewGuid().ToString("N") + ".tmp";
        lock (WriteLock)
        {
            try
            {
                if (document.Root == null)
                    throw new InvalidDataException("Le document à écrire ne contient aucun élément racine.");

                XDocument previous;
                try
                {
                    previous = XDocument.Load(fullPath);
                }
                catch (FileNotFoundException)
                {
                    previous = null;
                }

                if (previous != null)
                {
                    if (previous.Root.Name != document.Root.Name)
                        throw new InvalidDataException("L’élément racine diffère de celui du fichier actif.");

                    if (XNode.DeepEquals(previous, document))
                        return;
                }
                else if (File.Exists(backupPath))
                {
                    throw new IOException("Le fichier actif est absent alors qu’une copie de secours existe. Restauration manuelle requise.");
                }

                using (var stream = new FileStream(temporaryPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
                {
                    document.Save(stream);
                    stream.Flush(flushToDisk: true);
                }

                if (previous == null)
                    File.Move(temporaryPath, fullPath);
                else
                    File.Replace(temporaryPath, fullPath, backupPath);
            }
            catch (Exception error) when (error is IOException || error is UnauthorizedAccessException ||
                error is XmlException || error is InvalidDataException)
            {
                throw new IOException($"Impossible d’enregistrer le XML « {fullPath} ». Copie de secours éventuelle : « {backupPath} ».", error);
            }
            finally
            {
                try
                {
                    File.Delete(temporaryPath);
                }
                catch (Exception error) when (error is IOException || error is UnauthorizedAccessException)
                {
                    // Un temporaire non publié peut subsister si le système refuse son nettoyage.
                }
            }
        }
    }
}
