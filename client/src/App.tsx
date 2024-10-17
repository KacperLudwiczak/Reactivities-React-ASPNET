import { useEffect, useState } from "react";
import "./App.css";
import axios from "axios";

export default function App() {
  const [activities, setActivities] = useState([]);

  useEffect(() => {
    axios.get("http://localhost:5000/api/activities").then((response) => {
      setActivities(response.data);
    });
  }, []);

  return (
    <>
      <h1>Reactivities</h1>
      <ul>
        {activities.map((activity: any) => (
          <li key={activity.id}>{activity.title}</li>
        ))}
      </ul>
    </>
  );
}
