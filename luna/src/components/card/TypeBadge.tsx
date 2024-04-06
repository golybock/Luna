﻿import React from "react";
import TypeView from "../../models/card/view/typeView";
import "./TypeBadge.css";

interface IProps {
    type: TypeView
}

interface IState {
}

export default class TypeBadge extends React.Component<IProps, IState>{


    render() {
        return (
            <div className="Type-Container">
                <div className="Type" style={{background: this.props.type.color}}>
                    <span>{this.props.type.name}</span>
                </div>
            </div>
        );
    }
}