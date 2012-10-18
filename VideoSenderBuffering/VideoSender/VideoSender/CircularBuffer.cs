using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VideoSender
{
    class CircularBuffer
    {

        List<byte[]> buffer;
        int head;
        int tail;
        const int capacity = 200;
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
            this.buffer.Insert(tail, frame);

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
    }
}
