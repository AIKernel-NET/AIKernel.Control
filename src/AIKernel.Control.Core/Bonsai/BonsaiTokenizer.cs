using System.Text.Json;

namespace AIKernel.Control.Core.Bonsai;

/// <summary>[EN] Documents this public package API member. [JA] BonsaiTokenizer を表します。</summary>
/// <include file="docs.en.xml" path="doc/members/member[@name='T:AIKernel.Control.Core.Bonsai.BonsaiTokenizer']" />
/// <include file="docs.ja.xml" path="doc/members/member[@name='T:AIKernel.Control.Core.Bonsai.BonsaiTokenizer']" />
public sealed class BonsaiTokenizer
{
    private readonly Dictionary<string, int> _tokenToId;
    private readonly string[] _idToToken;

    private BonsaiTokenizer(
        Dictionary<string, int> tokenToId,
        string[] idToToken)
    {
        _tokenToId = tokenToId;
        _idToToken = idToToken;
    }

    /// <summary>[EN] Documents this public package API member. [JA] VocabularySize を取得します。</summary>
    /// <include file="docs.en.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiTokenizer.VocabularySize']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='F:AIKernel.Control.Core.Bonsai.BonsaiTokenizer.VocabularySize']" />
    public int VocabularySize => _idToToken.Length;

    /// <summary>[EN] Documents this public package API member. [JA] Parse を実行します。</summary>
    /// <include file="docs.en.xml" path="doc/members/member[@name='M:AIKernel.Control.Core.Bonsai.BonsaiTokenizer.Parse']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='M:AIKernel.Control.Core.Bonsai.BonsaiTokenizer.Parse']" />
    public static BonsaiTokenizer Parse(string json)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(json);

        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;
        var vocab = ResolveVocabulary(root);

        var tokenToId = new Dictionary<string, int>(StringComparer.Ordinal);
        var maxId = 0;

        foreach (var item in vocab.EnumerateObject())
        {
            if (item.Value.ValueKind != JsonValueKind.Number
                || !item.Value.TryGetInt32(out var id)
                || id < 0)
            {
                continue;
            }

            tokenToId[item.Name] = id;
            maxId = Math.Max(maxId, id);
        }

        if (tokenToId.Count == 0)
        {
            tokenToId["<unk>"] = 0;
            tokenToId["<eos>"] = 1;
            maxId = 1;
        }

        var idToToken = new string[maxId + 1];
        foreach (var pair in tokenToId)
        {
            idToToken[pair.Value] = pair.Key;
        }

        for (var i = 0; i < idToToken.Length; i++)
        {
            idToToken[i] ??= "<unk>";
        }

        return new BonsaiTokenizer(tokenToId, idToToken);
    }

    /// <summary>[EN] Documents this public package API member. [JA] TokenizeFirst を実行します。</summary>
    /// <include file="docs.en.xml" path="doc/members/member[@name='M:AIKernel.Control.Core.Bonsai.BonsaiTokenizer.TokenizeFirst']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='M:AIKernel.Control.Core.Bonsai.BonsaiTokenizer.TokenizeFirst']" />
    public int TokenizeFirst(ReadOnlySpan<char> text)
    {
        if (text.IsEmpty)
        {
            return 0;
        }

        foreach (var pair in _tokenToId)
        {
            if (text.SequenceEqual(pair.Key.AsSpan()))
            {
                return pair.Value;
            }
        }

        var hash = 2166136261u;
        foreach (var c in text)
        {
            hash ^= c;
            hash *= 16777619u;
        }

        return (int)(hash % (uint)Math.Max(1, _idToToken.Length));
    }

    /// <summary>[EN] Documents this public package API member. [JA] Decode を実行します。</summary>
    /// <include file="docs.en.xml" path="doc/members/member[@name='M:AIKernel.Control.Core.Bonsai.BonsaiTokenizer.Decode']" />
    /// <include file="docs.ja.xml" path="doc/members/member[@name='M:AIKernel.Control.Core.Bonsai.BonsaiTokenizer.Decode']" />
    public string Decode(int tokenId)
    {
        if ((uint)tokenId >= (uint)_idToToken.Length)
        {
            return "<unk>";
        }

        return _idToToken[tokenId];
    }

    private static JsonElement ResolveVocabulary(JsonElement root)
    {
        if (root.TryGetProperty("model", out var model)
            && model.ValueKind == JsonValueKind.Object
            && model.TryGetProperty("vocab", out var modelVocab)
            && modelVocab.ValueKind == JsonValueKind.Object)
        {
            return modelVocab;
        }

        if (root.TryGetProperty("vocab", out var vocab)
            && vocab.ValueKind == JsonValueKind.Object)
        {
            return vocab;
        }

        throw new InvalidDataException("Bonsai tokenizer JSON does not contain a vocab object.");
    }
}
