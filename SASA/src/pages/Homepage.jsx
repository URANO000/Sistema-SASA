//This one is for testing first
import React from "react";
import Dashboard from "../components/Dashboard";
import Breadcrumb from "../components/Breadcrumb";
import Table from "../components/Table";

function Home() {

    return (
        <div className="main-wrapper">
            <Breadcrumb />
            <div className="flex flex-row gap-4">
                <div className="card mb-0">
                    <h1>Home card</h1>
                </div>
                <div className="card mb-0">
                    <Dashboard />
                </div>
            </div>
            <div className="card mb-0">
                <Table />
            </div>
            <div className="card mb-0">
                <h1>Testing scroll</h1>

            </div>
        </div>

    )
};

export default Home;