using System;

namespace Lumley.AiTest.GameShared
{
    /// <summary>
    /// Pool for only blocks to allow serialization.
    /// </summary>
    [Obsolete("Use Pooling Manager instead")]
    public sealed class BlockPool : ObjectPool<Block>
    {
    }
}