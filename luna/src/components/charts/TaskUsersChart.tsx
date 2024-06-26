﻿import React from "react";
import {PieChart, PieValueType} from "@mui/x-charts";
import "./TaskStatesesChart.css";
import Form from "react-bootstrap/Form";
import {toDictionary, toUsersDictionary} from "../../models/tools/ModelsConverter";
import PieValue from "../../models/charts/PieValue";
import ICardView from "../../models/card/view/ICardView";

interface IProps {
    cards: ICardView[]
}

interface IState {
    series: PieValueType[],
    totalTasks: number
}

export default class TaskUsersChart extends React.Component<IProps, IState> {

    constructor(props: IProps) {
        super(props);

        this.state = {
            series: [],
            totalTasks: 0
        }
    }

    async componentDidMount() {
        const dict = toUsersDictionary(this.props.cards);

        let val : number = 0;
        let count: number = 0;

        const array: PieValue[] = [];

        dict.forEach(item => {
            val++;
            array.push({id: val, statusName: JSON.parse(item.user)?.username.toString(), count: item.card.length, color: "red"})
            count+= item.card.length;
        })

        const series = array.map(item => {
            return {value: item.count, label: item.statusName, id: item.id}
        })

        this.setState({series: series})
        this.setState({totalTasks: count})
        console.log(count)
    }

    componentDidUpdate(prevProps: Readonly<IProps>, prevState: Readonly<IState>, snapshot?: any) {
        const dict = toUsersDictionary(this.props.cards);

        let val : number = 0;
        let count: number = 0;

        const array: PieValue[] = [];

        dict.forEach(item => {
            val++;
            array.push({id: val, statusName: JSON.parse(item.user)?.username.toString(), count: item.card.length, color: "red"})
            count+= item.card.length;
        })

        const series = array.map(item => {
            return {value: item.count, label: item.statusName, id: item.id}
        })

        if(prevProps != this.props){
            this.setState({series: series})
            this.setState({totalTasks: count})
        }
    }

    render() {
        return (
            <div>
                <div className="Chart-Header">
                    <h2>{this.state.totalTasks} всего задач</h2>
                    {/*<Form.Select className="Date-Picker">*/}
                    {/*    <option value="1">1 Month</option>*/}
                    {/*    <option value="2">3 Months</option>*/}
                    {/*    <option value="3">1 Year</option>*/}
                    {/*</Form.Select>*/}
                </div>
                {this.state.series && (
                    <PieChart
                        colors={['orange', 'blue', 'green']}
                        series={[
                            {
                                data: this.state.series,
                                innerRadius: 80,
                                paddingAngle: 3,
                                cornerRadius: 5,
                            },
                        ]}
                        width={600}
                        height={250}/>
                )}
            </div>
        );
    }
}