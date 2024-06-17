﻿import React from "react";
import "./Home.css"
import {NavLink} from "react-router-dom";
import IPageView from "../../models/page/IPageView";
import WorkspaceProvider from "../../provider/workspace/workspaceProvider";
import {WorkspaceManager} from "../../tools/WorkspaceManager";
import PageProvider from "../../provider/page/pageProvider";

interface IProps {

}

interface IState {
    pages: IPageView[];
}

export default class Home extends React.Component<IProps, IState> {

    headerUrl ="http://localhost:7005/woodcuts_14.jpg";

    constructor(props: IProps) {
        super(props);

        this.state = {
            pages: []
        }
    }

    async componentDidMount() {
        let workspaceId = WorkspaceManager.getWorkspace();

        let pages = await PageProvider.getPages(workspaceId!);

        const sliced = pages.slice(0, 2);

        this.setState({pages: sliced})
    }


    render() {
        return (
            <div>
                <div className="Header-Image-Container">
                    <img src={this.headerUrl} alt=""/>
                </div>
                <div className="Home-Content">
                    <div>
                        <div className="Header">
                            <div>
                                <h1>Personal home</h1>
                                <label>Organize everything this</label>
                            </div>
                        </div>
                        <div className="Links">
                            <div className="Link-Block">
                                <h4>Daily</h4>
                                <hr/>
                                {this.state.pages && (
                                    this.state.pages.map((item) => {
                                       return (
                                           <NavLink to={"/page/" + item.id}>
                                                <p>{item.name}</p>
                                           </NavLink>
                                       )
                                    })
                                )}
                            </div>
                            <div className="Link-Block">
                                <h4>Management</h4>
                                <hr/>
                                <NavLink to="/statistic">
                                    <p>View statistic</p>
                                </NavLink>
                                <NavLink to="/settings">
                                    <p>Settings</p>
                                </NavLink>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}