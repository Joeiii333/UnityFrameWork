using System;

namespace TeachTcpServerExercises2
{
    class Program
    {
        public static ServerSocket serverSocket;
        static void Main(string[] args)
        {
            serverSocket = new ServerSocket();
            serverSocket.Start("127.0.0.1", 8080, 1024);
            Console.WriteLine("服务器开启成功");
            while (true)
            {
                string input = Console.ReadLine();
                if(input == "Quit")
                {
                    serverSocket.Close();
                }
                else if( input.Substring(0,2) == "B:" )
                {
                    serverSocket.Broadcast(input.Substring(2));
                }
            }
        }
    }
}
