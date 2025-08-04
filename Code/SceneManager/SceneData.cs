using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneData
{
    // Variables are to be serialized using static class
    public static Leveling leveling;
    public static List<string> Area1NormalZones { get; set; } = new();
    public static List<string> Area1BossZones { get; set; } = new();
    public static List<string> Area2NormalZones { get; set; } = new();
    public static List<string> Area2BossZones { get; set; } = new();
    public static List<string> Area3NormalZones { get; set; } = new();
    public static List<string> Area3BossZones { get; set; } = new();

    public static int ProgressionCounter { get; set; } = 0;
    public static int PlayerLevel { get; set; } = 0;
    
    // Player inventory
    public static List<string> PlayerInventory { get; set; } = new();
    
    // Set 
}
