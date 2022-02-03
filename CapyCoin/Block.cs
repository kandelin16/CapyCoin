using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using EllipticCurve;

namespace CapyCoin
{
    class Block
    {
        public int Index { get; set; }
        public string LastHash { get; set; }
        public string TimeStamp { get; set; }
        public string Hash { get; set; }
        public int Nonce { get; set; }
        public List<Transaction> Transactions { get; set; }

        public Block(int index, string timestamp, List<Transaction> transactions, string lasthash = "")
        {
            this.Index = index;
            this.TimeStamp = timestamp;
            this.Transactions = transactions;
            this.LastHash = lasthash;
            this.Hash = CalculateHash();
            this.Nonce = 0;
        }
        public string CalculateHash()
        {
            string tempData = Convert.ToString(this.Index) + this.LastHash + this.TimeStamp + this.Transactions.ToString() + this.Nonce;
            byte[] tempBytes = Encoding.ASCII.GetBytes(tempData);
            byte[] hash = SHA256.Create().ComputeHash(tempBytes);
            return BitConverter.ToString(hash).Replace("-", "");
        }
        public void Mine(int difficulty)
        {
            while (this.Hash.Substring(0,difficulty) != new String('0',difficulty))
            {
                this.Nonce++;
                this.Hash = this.CalculateHash();
                //Console.WriteLine("Mining: " + this.Hash);
            }
        }
    }
}
