using System.Collections;
using System.Collections.Generic;

public class StageInfo
{
    public static int maxLv = 5;
    public static List<string>[] stageRooms = new List<string>[]{
        // "Rnum|Pnum", Pnum > 0 means Person{Pnum} belongs Room{Rnum}, also means start point
        null, //stage 0 , means nothing
        new List<string>{ "1|0", "2|1" }, // stage 1
        new List<string>{ "1|2", "3|1", "4|0" }, // stage 2
        new List<string>{ "4|2", "5|5", "6|3", "7|4" }, // stage 3
        new List<string>{ "3|0", "4|0", "5|0", "6|0", "7|0", "8|0" }, //stage 4
        new List<string>{ "5|0", "6|4", "7|0", "8|6", "9|2", "10|3" }, //stage 5, last round
    };

    public static List<string>[] stageCondition = new List<string>[]
    {
        null, //stage 0 , means nothing
        new List<string>{ "p1|>|r1" }, //stage 1,
        new List<string>{ "p1|>|r4", "p2|>|r4", "p2|x|r3"}, //stage 2
        new List<string>{ "r4|c|r6", "p4|>|r6", "p2|x|p3", "p5|>|r4", "p2|>|r7" }, //stage 3
        new List<string>{ "r7|c|d2", "r3|c|d2", "r8|c|d2", "r4|c|d2", "r5|c|d2"}, //stage 4
        new List<string>{ "p6|>|r10", "p6|x|p2", "p2|>|r7", "p3|x|p4", "p3|>|r9",
                          "r9|x|r7", "r6|x|r5", "r6|c|d2" }, //stage 5
    };

}
