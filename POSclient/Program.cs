﻿using System;
using System.IO;
using System.Text;
using System.Net;
using System.Net.Sockets;
using Newtonsoft.Json;
using System.Collections.Generic;

class POSclient
{
    static void Main(string[] args)
    {
        TcpClient client = null;
        OrderList orderlist = new OrderList();
        try
        {
            //LocalHost에 지정포트로 TCP Connection생성 후 데이터 송수신 스트림 얻음
            client = new TcpClient();
            client.Connect("127.0.0.1", 5001);
            NetworkStream writeStream = client.GetStream();

            Encoding encode = System.Text.Encoding.GetEncoding("utf-8");
            StreamReader readerStream = new StreamReader(writeStream, encode);

            //("1 : 봉구스밥버거");
            //("2 : 김밥천국");
            //("3 : 봉봉치킨");
            //("4 : 맘스터치");
            Console.WriteLine("봉구스밥버거에 오신 것을 환영합니다");
            Console.WriteLine("4번 테이블 입니다.");
            //보낼 데이터를 읽어 Default형식의 바이트 스트림으로 변환

            string str = "{ \"restaurant_name\":\"봉구스밥버거\",\"restaurant_id\":1,\"table_num\":4}";
            str += "\r\n";
            byte[] data = Encoding.UTF8.GetBytes(str);
            writeStream.Write(data, 0, data.Length);
            int i = 1;


            // if (dataToSend.IndexOf("<EOF>") > -1)
            //  break;
            
            string menuData;
            menuData = readerStream.ReadLine();
            Console.WriteLine("JSON_DATA : " + menuData);
            //Menu menu = JsonConvert.DeserializeObject<Menu>(menuData);
            Menu[] menu_list = JsonConvert.DeserializeObject<Menu[]>(menuData);
            foreach (Menu m in menu_list)
            {
                Console.WriteLine(i + ". " + m.name + "\t\t|\t" + m.price + "원");
                i++;
            }
            
            while (true)
            {
                Console.WriteLine("1.주문하기 2.결제하기");
                int select_index = Convert.ToInt32(Console.ReadLine());
                if (select_index == 1)
                {
                    string menu_what = "0";
                    int menu_num = 0;
                    Console.WriteLine("메뉴 입력 : \"메뉴 수량\"");
                    
                    while (true)
                    {
                        Console.WriteLine("번호 : (주문완료 : -1)");
                        menu_what = (Console.ReadLine());
                        if (menu_what.Equals("-1"))
                        {
                            string menu_data_json = JsonConvert.SerializeObject(orderlist);
                            Console.WriteLine("menu_data_json: "+ menu_data_json);
                            menu_data_json += "\r\n";
                            data = Encoding.UTF8.GetBytes(menu_data_json);
                            writeStream.Write(data, 0, data.Length);
                            break;
                        }
                        menu_what=menu_list[Convert.ToInt32(menu_what)].name;
                        Console.WriteLine("수량 : ");
                        menu_num = Convert.ToInt32(Console.ReadLine());
                        orderlist.addMenu(menu_what, menu_num);
                    }
                }
                else if (select_index == 2)
                {
                    payment temp = new payment("1");
                    string menu_number_json = JsonConvert.SerializeObject(temp);
                    menu_number_json += "\r\n";
                    data = Encoding.UTF8.GetBytes(menu_number_json);
                    writeStream.Write(data, 0, data.Length);
                    break;
                }
            }
        }

        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        finally
        {
            client.Close();
        }
    }
}
