﻿using System.Linq;
using Google.Protobuf;
using Pericles.Transactions;

namespace Pericles.Blocks
{
    public class ProtoBlockFactory
    {
        private readonly ProtoTransactionFactory protoTransactionFactory;

        public ProtoBlockFactory(ProtoTransactionFactory protoTransactionFactory)
        {
            this.protoTransactionFactory = protoTransactionFactory;
        }

        public Protocol.Block Build(Block block)
        {
            var header = block.Header;
            var protoBlockHeader = new Protocol.BlockHeader
            {
                PrevBlockHash = ByteString.CopyFrom(header.PrevBlockHash.GetBytes()),
                MerkleRootHash = ByteString.CopyFrom(header.MerkleRootHash.GetBytes()),
                Timestamp = header.Timestamp,
                Bits = header.Bits,
                Nonce = header.Nonce
            };

            var protoBlock = new Protocol.Block
            {
                BlockHeader = protoBlockHeader,
                Hash = ByteString.CopyFrom(block.Hash.GetBytes()),
                VoteCounter = block.TransactionCounter,
                Votes = { }
            };

            var protoTransactions = block.MerkleTree.Votes.Select(x => this.protoTransactionFactory.Build(x));
            protoBlock.Votes.AddRange(protoTransactions);

            return protoBlock;
        }
    }
}