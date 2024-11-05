# Spineless Snake

This project is a modern twist on the classic Snake game, where the player controls a snake that collects items, avoiding walls and obstacles as they navigate the field. 

## Key Features and Changes

1. **Gameplay Adjustments**  
   - The snake **no longer collides with itself**. This makes the snake more focus on not hitting anything else and collect items.
   - **Walls as obstacles** are placed to prevent the user to achieve such a high score.

2. **Apple Replaced with Chicken**  
   - Instead of the original apple, it is a chicken for the snake to eat. 
  
3. **New Items**
   - **Dynamic Speed**: The snake’s speed increases progressively when it picks up the boots.
   - **Shrink**: Cuts off some body segments of the tail of the user's snake.
   - **Ball and Chain**: The snake's speed decreases when the user picks up this item.
   - **Map**: The field of view of the screen will be increased slightly 

5. **Scoring System**  
   - Points are based on the **total score** rather than just counting chickens collected.

6. **Field Redesign**  
   - The **background color** of the field has been updated, and **tiles have been removed** because the snake moves freely on the map.

7. **Bot Addition**  
   - Intention of the bot was to track the user down and try to get the user's snake head to run into the bot to end the game. As a workaround for not being able to solve the bot, it goes into a straight line and into walls where it will respawn everytime it hits a wall. It will also spawn into a different location so that it is unpredictable for the user. 

8. **Enhanced UI**  
   - **Opening and ending screens**.

## Future Enhancements
Hopefully, I can change the functionality of the bot in the future.
