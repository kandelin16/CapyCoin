using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using EllipticCurve;

namespace CapyCoin
{
    class Program
    {
        static void Main(string[] args)
        {
            PrivateKey key1 = new PrivateKey();
            PublicKey walletAddress1 = key1.publicKey();

            PrivateKey key2 = new PrivateKey();
            PublicKey walletAddress2 = key2.publicKey();

            Blockchain capycoin = new Blockchain(2, 10);

            Console.WriteLine("Mining initial Coin...");
            capycoin.MinePendingTransactions(walletAddress1);
            decimal balance = capycoin.GetBalanceOfWallet(walletAddress1);

            Transaction transaction1 = new Transaction(walletAddress1, walletAddress2, 3);
            transaction1.SignTransaction(key1);
            capycoin.AddTransaction(transaction1);

            Console.WriteLine("Mining Coin...");
            capycoin.MinePendingTransactions(walletAddress2);
            decimal balance2 = capycoin.GetBalanceOfWallet(walletAddress2);

            Console.WriteLine("Balance of Wallet 1 is: " + balance.ToString() + " capycoin.");
            Console.WriteLine("Balance of Wallet 2 is: " + balance2.ToString() + " capycoin.");

            //string blockJSON = JsonConvert.SerializeObject(capycoin, Formatting.Indented);
            //Console.WriteLine(blockJSON);

            //capycoin.GetLastestBlock().LastHash = "1234";
            if (capycoin.IsChainValid())
            {
                Console.WriteLine("Chain is Valid");
            }
            else Console.WriteLine("Chain is corrupted");
        }
    }
}
