/*
 * @lc app=leetcode id=1 lang=csharp
 *
 * [1] Two Sum
 */
public class Solution {
    public int[] TwoSum(int[] nums, int target) {
        //遍历的时候把不满足条件的放到一个字典，把数组的值作为key,把索引作为value放入value
        //如果目标值减去当前数组元素的值能在字典中找到，那字典中该key对应的value就是原来数组的索引
        //巧妙的是：通过字典，把原来数组元素作为字典key,把原来数组索引作为value,最终被取出
        Dictionary<int, int> tempDic = new Dictionary<int, int>();
        for(int i = 0; i < nums.Length; i++){
            if(tempDic.ContainsKey(target-nums[i])){
                return new int[] {tempDic[target-nums[i]], i};
            } else{
                tempDic[nums[i]] = i;
            }
        }
        return null;
    }
}

