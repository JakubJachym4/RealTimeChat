
    using System.Collections.Immutable;

    int[] nums1 = { 1, 2 };
    int[] nums2 = { 3, 4, 5 };

    double FindMedianSortedArrays(int[] nums1, int[] nums2)
    {
        if (nums1 == null && nums2 == null)
        {
            return 0;
        }
        var sorted = nums1.Concat(nums2).ToArray();
        Array.Sort(sorted); //wtf
        if (sorted.Length % 2 == 0)
        {
            return ((sorted[sorted.Length / 2 - 1] + sorted[sorted.Length / 2]) / 2.0);
        }

        return sorted[sorted.Length / 2];
    }
    
    
    Console.WriteLine(FindMedianSortedArrays(nums1, nums2));