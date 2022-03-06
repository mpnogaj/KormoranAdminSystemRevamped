import React from "react";
import { Table, Button } from "react-bootstrap";
import IMatch from "../Models/IMatch";
import ITeam from "../Models/ITeam";
import EditMatchRow from "./EditMatchRow";
import { nanoid } from "nanoid";

export interface IRowData{
	matchId: number,
	team1: number,
	team2: number,
	team1Score: number,
	team2Score: number;
}

interface IProps{
	matches: Array<IMatch>,
	teams: Array<ITeam>
}

interface IState {
	matchesData: Array<IRowData>;
}

class EditMatchTable extends React.Component<IProps, IState>{

	private DefaultMatchData() : IRowData {
		return {
			/*Id 0 will be changed to '-'*/
			matchId: 0,
			team1: 0,
			team2: 0,
			team1Score: 0,
			team2Score: 0
		}
	}

	constructor(props: IProps){
		super(props);
		this.state = {
			matchesData: [],
		};
	}

	componentDidMount(){
		const strippedMatches: Array<IRowData> = [];
		//append fetched data from serwer to local container
		this.props.matches.forEach(match => {
			strippedMatches.push({
				matchId: match.matchId,
				team1: match.team1Id,
				team2: match.team2Id,
				team1Score: match.team1Score,
				team2Score: match.team2Score
			});
		})
		this.setState({matchesData: strippedMatches});
	}
	
	render() {
		return(
			<div>
				<span>Mecze turnijowe: </span><br />
				<span>{}</span>
				<Button onClick={() => {
					this.setState(prevState => ({
						matchesData: [
							...prevState.matchesData, this.DefaultMatchData()
						]
					}), () => console.log(this.state.matchesData));
				}}>Dodaj nowy</Button>
				<Table className="mt-3" responsive={true} bordered={true}>
					<thead>
						<tr>
							<th>Id</th>
							<th>Drużyna 1</th>
							<th>Drużyna 2</th>
							<th>Wynik drużyny 1</th>
							<th>Wynik drużyny 2</th>
							<th>Zwyciezca</th>
							<th></th>
						</tr>
					</thead>
					<tbody>
						{
							this.state.matchesData.map((data: IRowData, index) => {
								return (
									<EditMatchRow 
										match={data}
										teams={this.props.teams}
										id={index} key={nanoid()}
										onUpdate={(targetId, targetVal, value) => {
											const newData: Array<IRowData> = this.state.matchesData.slice();
											switch(targetVal){
												case 1:
													newData[targetId].team1 = value;
													break;
												case 2:
													newData[targetId].team2 = value;
													break;
												case 3:
													newData[targetId].team1Score = value;
													break;
												case 4:
													newData[targetId].team2Score = value;
													break;
												case 5:
													//console.log(targetId);
													//console.log(newData);
													newData.splice(targetId, 1);
													//console.log(newData);
													break;
												default:
													console.error("Invalid targetVal parameter! Passed: " + targetVal);
													break;
											}
											this.setState({matchesData: newData});
										}}
									/>
								);
							})
						}
					</tbody>
				</Table>
			</div>
		)
	}
}

export default EditMatchTable