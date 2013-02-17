using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace VideoReceiver
{
    class CircularBuffer
    {
        List<byte[]> buffer;       
        int head;
        int tail;
        const int capacity = 1000;
        int count;
        

        //int turn;
        //bool[] flag;

        public CircularBuffer()
        {
            
            buffer = new List<byte[]>();
            for (int i = 0; i < capacity; i++)
                buffer.Add(null);           

            this.count = 0;
            this.head = this.tail = -1;         

            //this.turn = -1;
            //flag = new bool[2];
            //flag[0] = flag[1] = false;
        }

        public void enqueue(ref byte[] frame)
        {
            
            if (this.tail == -1 || this.tail == capacity - 1)
                this.tail = 0;
            else
                this.tail++;
            //Console.WriteLine("tail: " + this.tail);

            this.buffer[this.tail] = frame;
            //this.buffer.Insert(tail, frame);

            if (this.count > capacity)
                this.count = capacity / 2 + 1;
            else
                this.count++;
        }

        public int getHead()
        {
            return this.head;
        }

        public int getTail()
        {
            return this.tail;
        }

        public int diff()
        {
            //return ((capacity - 1) - this.head) + this.tail + 1;
            int size;
            if (this.tail >= this.head)
                size = (this.tail - this.head);
            else
                size = (this.tail + capacity - this.head);
            return size;  
        }

        public byte[] read()
        {
            
            if (this.head == -1 || this.head == capacity - 1)
                this.head = 0;
            else
                this.head++;

            //Console.WriteLine("head: " + this.head);            
            return this.buffer[this.head];
        }

        public int size()
        {
            //Console.WriteLine(count);
            return this.count;
        }

        /*public void enqueue(byte[] frame)
        {
            flag[0] = true;
            turn = 1;
            while (flag[1] && turn == 1) { }


            Console.WriteLine("enq");
            if (this.head == -1)
                this.head = this.tail = 0;
            else if (this.tail == capacity - 1)
                this.tail = 0;
            else
                this.tail++;

            buffer.Insert(tail,frame);
            this.count++;

            flag[0] = false;
            

        }*/

       /* public byte[] dequeue()
        {
            flag[1] = true;
            turn = 0;
            while (flag[0] && turn == 0)

            Console.WriteLine("deq");
            byte[] temp = buffer[head];

            if (this.head == this.tail)
                this.head = this.tail = -1;
            else if (this.head == capacity - 1)
                this.head = 0;
            else
                this.head++;
            
            this.count--;
            flag[1] = false;
            return temp;
            
        }*/

       

        /*public int size()
        {
            
            int size;
            if (this.head >= this.tail)
                size = (this.head - this.tail);
            else
                size = (this.head + capacity - this.tail);
            return size;            

        }*/
    }
}
