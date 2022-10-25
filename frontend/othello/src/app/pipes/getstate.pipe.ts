import { Pipe, PipeTransform } from "@angular/core";
import { getStartingState } from "../helper/functions";
import { game } from "../helper/models";





@Pipe({
    name: 'getState'
})
export class GetStatePipe implements PipeTransform{
    transform(currentGame: game | null) {
        if(!currentGame || currentGame.moves.length === 0) return getStartingState()
        else return currentGame.moves[currentGame.moves.length -1].resultingState
    }
}