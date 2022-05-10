## Setup and Required Packages
To run the virtual reality environment, the following hardware and software packages are required:

- `ROS2`: Our environment and plug-ins are compatible ROS2 Foxy or Galactic.
- `ROS-TCP-Enpoint Package`: ROS package used to create an endpoint to accept ROS messages sent from a Unity scene. https://github.com/Unity-Technologies/ROS-TCP-Endpoint
- `Oculus Integration Package`: Unity plug-in that enables hand tracking. https://assetstore.unity.com/packages/tools/integration/oculus-integration-82022
- `Oculus Quest 2`: The plug-ins used in this product is compatible with the Oculus Quest 2 virtual reality headset.


## How to Run Virtual Reality Environment

### Hardware and Unity
- Connect the Oculus Quest 2 to the local machine and enable Oculus Link.
- Open `arm_robot_test` Unity environment.
- Click “Robotics” the “ROS settings” at the top of the unity page and input your IP address.
- Click the child of the GameObject ROSconnection on the left of Unity environment and input your IP address.

### Start ROS Communication
- Run ROS2 in terminal
- Source the environment where TCP endpoint is situated
- Find your IP address using `ipconfig`
- Run the following command on your terminal to start the ROS communication in Unity: `ros2 run ros_tcp_endpoint default_server_endpoint --ros-args -p ROS_IP:=“YOUR IP ADDRESS”`

Once the VR headset is connected to the local machine, Unity is properly configured, and ROC-TCP-Endpoint package is running in the terminal, hit the play button at the top of the Unity scene to start the simulation. Note that the Gazebo "hardware simulation" environment must also be running and subscribed to the proper topics.  


