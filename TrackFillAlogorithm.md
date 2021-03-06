## 圈地算法 Track Fill Algorithm

# 需求
现在游戏市场以“球球大作战”为代表的io游戏如此火爆，是时候分析一些经典的io游戏了！<br>
如果你手上正在做此类型的游戏，希望这篇文章能给你一些参考价值;<br>
如果你目前没有做此类型的游戏，那么有备无患，将来也许用得上。<br>
![example](https://github.com/RickJiangShu/Documents/blob/master/TrackFillAlgorithm/example.gif "example")<br>
<br>
# 演示项目
https://github.com/RickJiangShu/TrackFillAlgorithm-Example<br>
<br>
# 目的
本文章将解析上面游戏中的“圈地算法”，即在二维格子坐标下，给定任意形状的图形，完成其填充。<br>
<br> 
# 算法难点
* 不同于“颜料桶”填充算法，在二维格子的条件下，会产生多个封闭区域。如下图1.1<br>
![1-1](https://github.com/RickJiangShu/Documents/blob/master/TrackFillAlgorithm/1-1.jpg "1-1")<br>
解决方案：通过横线/竖线检测，对多个区域进行填充。
  
  
* 两点构成一线，竖线连竖线/横线连横线很容易理解，那么遇到拐角怎么算呢？如下图1-2
![1-2](https://github.com/RickJiangShu/Documents/blob/master/TrackFillAlgorithm/1-2.jpg "1-2")  
  
解决方案：通过观察所有的角(图1-3)并总结其规律  
![1-3](https://github.com/RickJiangShu/Documents/blob/master/TrackFillAlgorithm/1-3.jpg "1-3")  
  
  
判断拐角规律如下：  
```
private bool IsHorizontalPoint(Grid grid)
{
    if (grid.trackFlag == TrackFlag.VerticalTrack || grid.trackFlag == TrackFlag.TrackCorner1 || grid.trackFlag == TrackFlag.TrackCorner2)
        return true;

    if (grid.trackFlag == TrackFlag.TrackCorner3 && FindRightCornerOrGround(grid, TrackFlag.TrackCorner2))
        return true;

    if (grid.trackFlag == TrackFlag.TrackCorner0 && FindRightCornerOrGround(grid, TrackFlag.TrackCorner1))
        return true;

    return false;
}

private bool IsVerticalPoint(Grid grid)
{
    if (grid.trackFlag == TrackFlag.HorizontalTrack
        || grid.trackFlag == TrackFlag.TrackCorner0
        || grid.trackFlag == TrackFlag.TrackCorner1)
        return true;

    if (grid.trackFlag == TrackFlag.TrackCorner2 && FindDownCornerOrGround(grid, TrackFlag.TrackCorner1))
        return true;

    if (grid.trackFlag == TrackFlag.TrackCorner3 && FindDownCornerOrGround(grid, TrackFlag.TrackCorner0))
        return true;

    return false;
}
```
  
判断结果如下图1-4：    
![1-4](https://github.com/RickJiangShu/Documents/blob/master/TrackFillAlgorithm/1-4.jpg "1-4")  


# 算法流程
1、逐列进行“横向检测”，将框线两条竖线之间的格子标记为1;  
2、逐行进行“纵向检测”，将框线两条横线之间的格子标记为1,如下图1-5  
![1-5](https://github.com/RickJiangShu/Documents/blob/master/TrackFillAlgorithm/1-5.jpg "1-5")  
  
3、遍历找寻种子，并进行种子填充，将种子标记为2，如下图1-6;  
![1-6](https://github.com/RickJiangShu/Documents/blob/master/TrackFillAlgorithm/1-6.jpg "1-6")  
  
4、填充所有标记为2的格子和框线。  


# 关键代码
```
private int[,] fillTestMap;//1 表示通过竖向检测
private void TestAndFill()
{
    fillTestMap = new int[grids.Width, grids.Height];

    HorizontalTest();//横线检测

    VerticalTest();//竖线检测

    SeedTest();//种子注入

    FillGrids();//填充
}

private void HorizontalTest()
{
    for (int y = topLeft[1]; y <= bottomRight[1]; y++)
    {
        List<Grid> points = new List<Grid>();//横向的点
        int groundIndex = -1;

        for (int x = topLeft[0]; x <= bottomRight[0];x++)
        {
            Grid grid = grids[x, y];
            if (IsMyTrack(grid) && IsHorizontalPoint(grid))
            {
                points.Add(grid);
            }
            else if (groundIndex == -1 && IsMyGround(grid))
            {
                groundIndex = points.Count;
            }
        }


        //奇数，加入Ground
        int count = points.Count;
        if (count % 2 != 0)
        {
            if (groundIndex == -1)
            {
                continue;//“分割”的情况，只有一条“线”
            }

            Grid linkPoint;
            Grid linkGround;
            //奇数为终点，向右找
            if (groundIndex % 2 != 0)
            {
                linkPoint = points[groundIndex - 1];
                linkGround = FindGround(linkPoint.x, linkPoint.y, 1, 0);
            }
            //偶数为起点，向右找
            else
            {
                linkPoint = points[groundIndex];
                linkGround = FindGround(linkPoint.x, linkPoint.y, -1, 0);
            }
            points.Insert(groundIndex, linkGround);
        }

        for (int i = 0; i < count; i += 2)
        {
            HorizontalLink(points[i], points[i + 1]);
        }
    }
}

private void VerticalTest()
{
    for (int x = topLeft[0]; x <= bottomRight[0]; x++)
    {
        List<Grid> points = new List<Grid>();
        int groundIndex = -1;

        for (int y = topLeft[1]; y <= bottomRight[1]; y++)
        {
            Grid grid = grids[x, y];
            if (IsMyTrack(grid) && IsVerticalPoint(grid))
            {
                points.Add(grid);
            }
            else if (groundIndex == -1 && IsMyGround(grid))
            {
                groundIndex = points.Count;
            }
        }

        //奇数，加入Ground
        int count = points.Count;
        if (count % 2 != 0)
        {
            if (groundIndex == -1)
            {
                continue;//“分割”的情况，只有一条“线”
            }

            Grid linkPoint;
            Grid linkGround;
            //奇数
            if (groundIndex % 2 != 0)
            {
                linkPoint = points[groundIndex - 1];
                linkGround = FindGround(linkPoint.x, linkPoint.y, 0, 1);
            }
            //偶数
            else
            {
                linkPoint = points[groundIndex];
                linkGround = FindGround(linkPoint.x, linkPoint.y, 0, -1);
            }

            points.Insert(groundIndex, linkGround);
        }

        for (int i = 0; i < count; i += 2)
        {
            VerticalLink(points[i], points[i + 1]);
        }
    }
        
}

private void SeedTest()
{
    CurrentSeedFlag = SeedFlagStart;
    SeedClosedFlags = new List<int>();

    for (int y = topLeft[1] + 1; y < bottomRight[1]; y++)
    {
        for (int x = topLeft[0] + 1; x < bottomRight[0]; x++)
        {
            if (fillTestMap[x, y] == 1)
            {
                if (SeedFill(grids[x, y]) == false)
                {
                    CurrentSeedFlag++;
                    continue;
                }

                SeedClosedFlags.Add(CurrentSeedFlag++);
            }
        }
    }
}

private void FillGrids()
{
    //轨迹格子 
    foreach (Grid g in allTrackGrids)
    {
        Map.SetGround(g.x, g.y, this,false);
    }

    //种子填充
    for (int y = topLeft[1] + 1; y < bottomRight[1]; y++)
    {
        for (int x = topLeft[0] + 1; x < bottomRight[0]; x++)
        {
            int flag = fillTestMap[x, y];
            if (flag >= SeedFlagStart && SeedClosedFlags.Contains(flag))
            {
                Grid g = grids[x, y];
                Map.SetGround(g.x, g.y, this,false);
            }
        }
    }
}
```
