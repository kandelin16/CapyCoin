using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using EllipticCurve;

namespace CapyCoin
{
    class Transaction
    {
        public PublicKey FromAddress { get; set; }
        public PublicKey ToAddress { get; set; }
        public decimal Amount { get; set; }
        public Signature Signature { get; set; }
        public Transaction(PublicKey fromAddress, PublicKey toAddress, decimal amount)
        {
            this.FromAddress = fromAddress;
            this.ToAddress = toAddress;
            this.Amount = amount;
        }
        public void SignTransaction(PrivateKey signingKey)
        {
            string fromAddressDER = BitConverter.ToString(this.FromAddress.toDer()).Replace("-", "");
            string signingDER = BitConverter.ToString(signingKey.publicKey().toDer()).Replace("-", "");
        
            if (fromAddressDER != signingDER)
            {
                throw new Exception("You cannot sign another wallets transactions");
            }

            string txHash = this.CalculateHash();
            this.Signature = Ecdsa.sign(txHash, signingKey);
        }
        public string CalculateHash()
        {
            string fromAddressDER = BitConverter.ToString(this.FromAddress.toDer()).Replace("-", "");
            string toAddressDER = BitConverter.ToString(this.FromAddress.toDer()).Replace("-", "");
            string transactionData = fromAddressDER = toAddressDER + this.Amount;
            byte[] tdBytes = Encoding.ASCII.GetBytes(transactionData);
            return BitConverter.ToString(SHA256.Create().ComputeHash(tdBytes)).Replace("-", "");
        }
        public bool IsValid()
        {
            if (this.FromAddress is null) return true;

            if (this.Signature is null)
            {
                throw new Exception("No signature in the transaction");
            }
            return Ecdsa.verify(this.CalculateHash(), this.Signature, this.FromAddress);
        }
    }
}
