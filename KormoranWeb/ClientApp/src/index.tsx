import { render } from "react-dom";
import {
	BrowserRouter,
	Routes,
	Route,
} from "react-router-dom";
import "./index.css";
import "bootstrap/dist/js/bootstrap.bundle.min";
import React from "react";
import App from "./App";
import Guest from "./Pages/Guest/Guest";
import Login from "./Pages/Login/Login";
import Panel from "./Pages/Panel/Panel";
import ProtectedRoute from "./Components/ProtectedRoute";
import Tournaments from "./Pages/Panel/Tournaments/Tournaments";
import Overview from "./Pages/Panel/Overview/Overview";
import Users from "./Pages/Panel/Users/Users";
import Logs from "./Pages/Panel/Logs/Logs";
import EditTournament from "./Pages/Panel/Tournaments/EditTournament";
import axios from "axios";

axios.defaults.withCredentials = true;

render(
	<BrowserRouter>
		<Routes>
			<Route path="/" element={<App />} />
			<Route path="Login" element={<Login />} />
			<Route path="Guest" element={<Guest />} />
			<Route path="Panel" element={<ProtectedRoute />}>
				<Route path="" element={<Panel content={<Overview />} />} />
				<Route path="Tournaments" element={<Panel content={<Tournaments />} />} />
				<Route path="EditTournament/:id" element={<Panel content={<EditTournament />} />} />
				<Route path="Overview" element={<Panel content={<Overview />} />} />
				<Route path="Users" element={<Panel content={<Users />} />} />
				<Route path="Logs" element={<Panel content={<Logs />} />} />
			</Route>
		</Routes>
	</BrowserRouter>,
	document.getElementById("root")
);