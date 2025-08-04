using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class NotificationManager : MonoBehaviour
{
    public TextMeshProUGUI notificationText; // Drag your UI Text component here
    public float displayDuration = 5f; // How long each notification will be displayed
    public float fadeDuration = 1f; // How long it takes to fade out the notification

    private Queue<Notification> notificationQueue = new Queue<Notification>(); // Queue to hold notifications
    private Coroutine currentNotificationCoroutine;

    private Dictionary<string, int> itemPickupTracker = new Dictionary<string, int>();

    private class Notification
    {
        public string itemName;
        public int quantity;

        public Notification(string itemName, int quantity)
        {
            this.itemName = itemName;
            this.quantity = quantity;
        }
    }

    private void Start()
    {
        if (notificationText == null)
        {
            //Debug.LogError("Notification Text is not assigned.");
        }
    }
    
    // Add a notification for each item and quantity
    public void ShowNotification(string itemName, int quantity)
    {
        // Add to the tracker to accumulate total quantity for each item
        if (itemPickupTracker.ContainsKey(itemName))
        {
            itemPickupTracker[itemName] += quantity;
        }
        else
        {
            itemPickupTracker[itemName] = quantity;
        }

        // We will not display any notification until all items are processed
        if (currentNotificationCoroutine == null)
        {
            currentNotificationCoroutine = StartCoroutine(DisplayNotificationsAfterProcessing());
        }
    }

    private IEnumerator DisplayNotificationsAfterProcessing()
    {
        // Wait a frame or two to ensure all items are picked up first
        yield return new WaitForSeconds(0.4f);

        foreach (var item in itemPickupTracker)
        {
            // Create a notification for each item with the correct total quantity
            notificationQueue.Enqueue(new Notification(item.Key, item.Value));
        }

        // Now start displaying all the notifications in sequence
        StartCoroutine(DisplayNotifications());

        // Clear the item pickup tracker after notifications are queued
        itemPickupTracker.Clear();
    }

    private IEnumerator DisplayNotifications()
    {
        while (notificationQueue.Count > 0)
        {
            Notification notification = notificationQueue.Dequeue();
            notificationText.text = $"Picked up {notification.quantity} {notification.itemName}";
            //Debug.Log($"Displaying notification for: {notification.itemName}, Quantity: {notification.quantity}");

            notificationText.color = new Color(notificationText.color.r, notificationText.color.g, notificationText.color.b, 1);

            // Display notification for the set duration
            yield return new WaitForSeconds(displayDuration);

            // Fade out the notification
            float elapsedTime = 0;
            Color startColor = notificationText.color;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
                notificationText.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
                yield return null;
            }

            notificationText.text = ""; // Clear text after fade-out
        }

        currentNotificationCoroutine = null; // Reset coroutine reference
    }
}
