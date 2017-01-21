using System;
using System.IO;
using System.Linq;

namespace SupervisedPassword
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length < 2 || !File.Exists(args[0]) || !File.Exists(args[1]))
            {
                Console.WriteLine("Enter existing file with passwords and file with alphabets");
                Environment.Exit(0);
            }
            var passwords = File.ReadAllLines(args[0]).Select(x => x.Split(' ')).ToDictionary(x => x[0], x => x[1]);

            var lines = File.ReadAllLines(args[1]);

            var enAlph = lines[0].ToCharArray();
            var mainAlph = lines[1].ToCharArray().Except(enAlph).ToArray();
            //todo generate hit
            var random = new Random(Guid.NewGuid().GetHashCode());
            var nums = new[] {2, 3, 4};
            while (true)
            {
                Console.Write("Login: ");
                var login = Console.ReadLine();

                if (!passwords.ContainsKey(login))
                {
                    Console.WriteLine("Login is incorrect");
                }
                else
                {
                    var pwd = passwords[login];
                    var len = random.Next(0, pwd.Length + 1);
                    var num = nums[random.Next(0, 3)];
                    var resultLength = pwd.Length*num;
                    var str = new char[resultLength];
                    for (var i = 0; i < resultLength - len; ++i)
                    {
                        str[i] = mainAlph[random.Next(0, mainAlph.Length)];
                    }

                    for (var i = resultLength - len; i < resultLength; ++i)
                    {
                        str[i] = enAlph[random.Next(0, enAlph.Length)];
                    }

                    for (var i = resultLength - 1; i >= 0; i--)
                    {
                        var tmp = str[i];
                        var randomIndex = random.Next(i + 1);

                        //Swap elements
                        str[i] = str[randomIndex];
                        str[randomIndex] = tmp;
                    }

                    Console.WriteLine(string.Join("", str));
                    var enteredPwd = Console.ReadLine();
                    pwd = pwd.Length <= enAlph.Length
                        ? pwd
                        : string.Join("", Enumerable.Repeat(pwd, (enAlph.Length/pwd.Length) + 1));
                    var flag = true;
                    for (var i = 0; i < resultLength; ++i)
                    {
                        if (enAlph.Contains(str[i]))
                        {
                            var idx = enAlph.Select((j, k) => Tuple.Create(j, k)).First(x => x.Item1 == str[i]).Item2;
                            var symb = pwd[idx];
                            if (symb != enteredPwd[i])
                            {
                                Console.WriteLine("Incorrect password");
                                flag = false;
                                break;
                            }
                        }else if (!mainAlph.Contains(enteredPwd[i]))
                        {
                            Console.WriteLine("Incorrect password");
                            flag = false;
                            break;
                        }
                    }

                    if (flag)
                    {
                        Console.WriteLine("Password is correct!");
                    }
                }
            }
        }
    }
}