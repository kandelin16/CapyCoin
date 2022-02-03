using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using EllipticCurve;

namespace CapyCoin
{
    class Blockchain
    {
        public List<Block> Chain { get; set; }
        public int Difficulty { get; set; }
        public List<Transaction> PendingTransactions { get; set; }
        public decimal MiningReward { get; set; }
        public Blockchain(int difficulty, decimal reward)
        {
            this.Chain = new List<Block>();
            this.Chain.Add(CreateGenesisBlock());
            this.Difficulty = difficulty;
            this.MiningReward = reward;
            this.PendingTransactions = new List<Transaction>();
        }
        public Block CreateGenesisBlock()
        {
            return new Block(0, DateTime.Now.ToString("yyyyMMddHHmmssffff"), new List<Transaction>());
        }
        public Block GetLastestBlock()
        {
            return this.Chain.Last();
        }
        public void AddBlock(Block newBlock)
        {
            newBlock.LastHash = this.GetLastestBlock().Hash;
            newBlock.Hash = newBlock.CalculateHash();
            this.Chain.Add(newBlock);
        }
        public void AddTransaction(Transaction transaction)
        {
            if (transaction.FromAddress is null || transaction.ToAddress is null)
            {
                throw new Exception("Transaction must include a to and from address.");
            }
            if (transaction.Amount > this.GetBalanceOfWallet(transaction.FromAddress))
            {
                throw new Exception("Insufficient Funds in sending wallet.");
            }
            if (!transaction.IsValid())
            {
                throw new Exception("Cannot add an invalid transaction!");
            }
            this.PendingTransactions.Add(transaction);
        }
        public decimal GetBalanceOfWallet(PublicKey address)
        {
            decimal balance = 0;

            string addressDER = BitConverter.ToString(address.toDer()).Replace("-", "");

            foreach (Block block in this.Chain)
            {
                foreach (Transaction transaction in block.Transactions)
                {
                    
                    string toDER = BitConverter.ToString(transaction.ToAddress.toDer()).Replace("-", "");
                    if (!(transaction.FromAddress is null))
                    {
                        string fromDER = BitConverter.ToString(transaction.FromAddress.toDer()).Replace("-", "");
                        if (fromDER == addressDER)
                        {
                            balance -= transaction.Amount;
                        }
                    }
                    if (toDER == addressDER)
                    {
                        balance += transaction.Amount;
                    }
                }
            }
            return balance;
        }
        public bool IsChainValid()
        {
            for (var i = 1; i < this.Chain.Count; i++)
            {
                //does hash match previous block hash
                Block currentBlock = this.Chain[i];
                if (currentBlock.LastHash != this.Chain[i - 1].Hash || currentBlock.Hash != currentBlock.CalculateHash())
                {
                    return false;
                }
            }
            return true;
        }
        public void MinePendingTransactions(PublicKey minerKey)
        {
            Transaction reward = new Transaction(null, minerKey, this.MiningReward);
            this.PendingTransactions.Add(reward);

            Block newBlock = new Block(this.GetLastestBlock().Index + 1, DateTime.Now.ToString("yyyyMMddHHmmssffff"), this.PendingTransactions, this.GetLastestBlock().Hash);
            newBlock.Mine(this.Difficulty);
            Console.WriteLine("Block successfully mined! Hash: " + newBlock.Hash);
            this.Chain.Add(newBlock);
            this.PendingTransactions = new List<Transaction>();
        }
    }
}
